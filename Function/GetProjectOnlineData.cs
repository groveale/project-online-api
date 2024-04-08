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
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // the solution uses app reg / client secret to get the contents of the keyVault
            // It would be better to use Managed Identity to access the keyVault

            var settings = Settings.LoadSettings();

            // for local dev
            var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName, settings.KeyVaultClientId, settings.KeyVaultClientSecret, settings.KeyVaultTenantId);

            // for Azure
            //var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName);

            var authHelper = new AuthenticationHelper(settings, keyVaultHelper);

            var accessToken = await authHelper.GetAccessToken();

             // Every item in the pull with have the same snapshot date
            var snapshot = DateTime.Now;

            var projectHelper = new ProjectOnlineHelper(settings.ProjectOnlineSiteUrl, accessToken, DateTime.Now.AddMinutes(50), log, snapshot, settings.FullPull, authHelper);

            //var projectData = await projectHelper.GetProjectData();

           

            var sqlHelper = new SqlHelper(settings.SqlConnectionString, projectHelper._compositeKeys);
            projectHelper._sqlHelper = sqlHelper;

            // Dictionary of tables with rows to insert
            Dictionary<string, int> additionalRows = new Dictionary<string, int>();


            additionalRows.Add("Projects", await projectHelper.GetProjects());
            additionalRows.Add("ProjectBaselines", await projectHelper.GetProjectsBaseline());
            additionalRows.Add("Tasks", await projectHelper.GetTasks());
            additionalRows.Add("TaskBaselines", await projectHelper.GetTaskBaseline());
            additionalRows.Add("Assignments", await projectHelper.GetAssignments());
            additionalRows.Add("AssignmentBaselines", await projectHelper.GetAssignmentBaseline());
            additionalRows.Add("Resources", await projectHelper.GetResources());
            
            return new OkObjectResult(additionalRows);
        }

    }
}
