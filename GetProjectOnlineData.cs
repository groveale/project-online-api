using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            refreshToken = refreshToken ?? data?.name;

            var settings = Settings.LoadSettings();

            var authHelper = new AuthenticationHelper(settings.ClientId, settings.ClientSecret, refreshToken, settings.Scope, settings.TenantId);

            var accessToken = await authHelper.GetAccessToken();

            var projectHelper = new ProjectOnlineHelper(settings.ProjectOnlineSiteUrl, accessToken, DateTime.Now.AddHours(1));

            var projectDate = projectHelper.GetProjects();

            return new OkObjectResult("Yay");
        }
    }
}
