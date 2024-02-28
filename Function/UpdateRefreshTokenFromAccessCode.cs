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
    public static class UpdateRefreshTokenFromAccessCode
    {
        [FunctionName("UpdateRefreshTokenFromAccessCode")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string code = req.Query["code"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            code = code ?? data?.code;

            try {
                var settings = Settings.LoadSettings();
                // for local dev
                //var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName, settings.KeyVaultClientId, settings.KeyVaultClientSecret, settings.KeyVaultTenantId);
                // for Azure
                var keyVaultHelper = new KeyVaultHelper(settings.KeyVaultName);
                var authHelper = new AuthenticationHelper(settings, keyVaultHelper);

                var refreshToken = await authHelper.UpdateRefreshTokenFromAuthCode(code);

                return new OkObjectResult($"Successfully updated refreshToken: {refreshToken}");
            } catch (Exception ex) {
                return new BadRequestObjectResult(ex.Message);
            }

            
        }
    }
}
