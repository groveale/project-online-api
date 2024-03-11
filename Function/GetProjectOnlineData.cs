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
            //var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName, settings.KeyVaultClientId, settings.KeyVaultClientSecret, settings.KeyVaultTenantId);

            // for Azure
            var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName);

            var authHelper = new AuthenticationHelper(settings, keyVaultHelper);

            var accessToken = await authHelper.GetAccessToken();

            var projectHelper = new ProjectOnlineHelper(settings.ProjectOnlineSiteUrl, accessToken, DateTime.Now.AddMinutes(50), log, settings.FullPull, authHelper);

            //var projectData = await projectHelper.GetProjectData();

            var sqlHelper = new SqlHelper(settings.SqlConnectionString, projectHelper._compositeKeys);

            // Dictionary of tables with rows to insert
            Dictionary<string, int> additionalRows = new Dictionary<string, int>();

            // Every item in the pull with have the same snapshot date
            var now = DateTime.Now;

            additionalRows.Add("Projects", InsertData(await projectHelper.GetProjects(), sqlHelper, now, "Projects", log));
            additionalRows.Add("ProjectBaselines", InsertData(await projectHelper.GetProjectsBaseline(), sqlHelper, now, "ProjectBaselines", log));
            additionalRows.Add("Tasks", InsertData(await projectHelper.GetTasks(), sqlHelper, now, "Tasks", log));
            additionalRows.Add("TaskBaselines", InsertData(await projectHelper.GetTaskBaseline(), sqlHelper, now, "TaskBaselines", log));
            additionalRows.Add("Assignments", InsertData(await projectHelper.GetAssignments(), sqlHelper, now, "Assignments", log));
            additionalRows.Add("AssignmentBaselines", InsertData(await projectHelper.GetAssignmentBaseline(), sqlHelper, now, "AssignmentBaselines", log));
            additionalRows.Add("Resources", InsertData(await projectHelper.GetResources(), sqlHelper, now, "Resources", log));
            
            return new OkObjectResult(additionalRows);
        }
    
    
        // method to insert the data into the database
        public static int InsertData(List<JObject> projectObjectData, SqlHelper sqlHelper, DateTime now, string objectKey, ILogger log)
        {   
            int rowsAffected = 0;
            log.LogInformation("Processing " + objectKey + " data");

            foreach(var item in projectObjectData)
            {
                // add snapshot date to each project object
                item["SnapshotDate"] = now;

                try {
                    sqlHelper.AddObjectToTable(item, objectKey);
                    rowsAffected++;
                }
                catch (Exception ex)
                {
                    // SQL error
                    log.LogError(ex.Message);
                }
            }

            return rowsAffected;
        }

    }
}
