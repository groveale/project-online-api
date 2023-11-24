using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace groveale
{
    public static class GetProjectOnlineData
    {
        [FunctionName("GetProjectOnlineData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // refreshToken in a keyVault
            // clientSecret and SQL connection string should also get moved to keyVault. Currently in settings

            // the solution uses app reg / client secret to get the contents of the keyVault
            // It would be better to use Managed Identity to access the keyVault

            var settings = Settings.LoadSettings();

            var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName, settings.KeyVaultClientId, settings.KeyVaultClientSecret, settings.KeyVaultTenantId);

            var authHelper = new AuthenticationHelper(settings.ClientId, settings.ClientSecret, settings.Scope, settings.TenantId, keyVaultHelper);

            var accessToken = await authHelper.GetAccessToken();

            var projectHelper = new ProjectOnlineHelper(settings.ProjectOnlineSiteUrl, accessToken, DateTime.Now.AddHours(1), settings.FullPull, authHelper);

            var projectData = await projectHelper.GetProjects();

            var sqlHelper = new SqlHelper(settings.SqlConnectionString);

            int rowsAffected = 0;

            foreach(var project in projectData)
            {
                // add snapshot date to each project object
                project["SnapshotDate"] = DateTime.Now;

                // get the content related to the project (as the project has been modified we can get the Tasks, Baselines and Assignments)
                // ToDO

                try {
                    sqlHelper.AddObjectToTable(project, "Projects");
                    rowsAffected++;
                }
                catch (Exception ex)
                {
                    // SQL error
                    log.LogError(ex.Message);
                }
            }
            
            return new OkObjectResult($"{rowsAffected} rows affected");
        }
    }
}
