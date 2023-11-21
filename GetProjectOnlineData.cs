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

            // refreshToken and client secret will end up in a keyVault - but for now are parameters
            string refreshToken = req.Query["refreshToken"];
            string deltaPull = req.Query["deltaPull"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            refreshToken = refreshToken ?? data?.name;
            deltaPull = deltaPull ?? data?.deltaPull;

            var settings = Settings.LoadSettings();

            var authHelper = new AuthenticationHelper(settings.ClientId, settings.ClientSecret, refreshToken, settings.Scope, settings.TenantId);

            var accessToken = await authHelper.GetAccessToken();

            var projectHelper = new ProjectOnlineHelper(settings.ProjectOnlineSiteUrl, accessToken, DateTime.Now.AddHours(1), settings.Debug);

            var projectData = await projectHelper.GetProjects();

            // Go and get other data

            var sqlHelper = new SqlHelper(settings.SqlConnectionString);

            // NEed a list of objects rather than an object that contains a list

            int rowsAffected = 0;

            foreach(var project in projectData)
            {
                // add snapshot date to each project object
                project["SnapshotDate"] = DateTime.Now;

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
            //return new OkObjectResult(projectData);
        }
    }
}
