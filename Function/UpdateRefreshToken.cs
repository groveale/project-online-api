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
    public static class UpdateRefreshToken
    {
        private const bool EnableFunction = false;

        [FunctionName("UpdateRefreshToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (!EnableFunction)
            {
                log.LogInformation("Function is disabled.");
                return new BadRequestObjectResult("Function is disabled");
            }

            log.LogInformation("C# HTTP trigger function processed a request.");

            string refreshToken = req.Query["refreshToken"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            refreshToken = refreshToken ?? data?.refreshToken;

            var settings = Settings.LoadSettings();

            var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName, settings.KeyVaultClientId, settings.KeyVaultClientSecret, settings.KeyVaultTenantId);

            try 
            {
                keyVaultHelper.SetSecret("SPOProjectOnlineRefreshToken", refreshToken);
                // Return a success message
                return new OkObjectResult("Refresh token updated");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
