using System;

namespace groveale
{
    public class Settings
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TenantId { get; set; }
        public string? ProjectOnlineSiteUrl { get;set;}
        public string? Scope { get; set; }
        public bool FullPull { get; set; }
        public string? SqlConnectionString { get; set; }
        public string? KeyVaultName { get; set; }
        public string? KeyVaultClientId { get; set; }
        public string? KeyVaultClientSecret { get; set; }
        public string? KeyVaultTenantId { get; set; }
        public string? RedirectUri { get; set; }

        public static Settings LoadSettings()
        {
            return new Settings 
            {
                ClientId = Environment.GetEnvironmentVariable("clientId"),
                ClientSecret = Environment.GetEnvironmentVariable("clientSecret"),
                TenantId = Environment.GetEnvironmentVariable("tenantId"),
                ProjectOnlineSiteUrl = Environment.GetEnvironmentVariable("projectOnlineSiteUrl"),
                Scope = Environment.GetEnvironmentVariable("scope"),
                FullPull = Environment.GetEnvironmentVariable("fullPull") == "true",
                SqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString"),
                KeyVaultName = Environment.GetEnvironmentVariable("keyVaultName"),
                KeyVaultClientId = Environment.GetEnvironmentVariable("keyVaultClientId"),
                KeyVaultClientSecret = Environment.GetEnvironmentVariable("keyVaultClientSecret"),
                KeyVaultTenantId = Environment.GetEnvironmentVariable("keyVaultTenantId"),
                RedirectUri = Environment.GetEnvironmentVariable("redirectUri")           
            };
        }
    }
}