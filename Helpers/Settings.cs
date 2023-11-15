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

        public static Settings LoadSettings()
        {
            return new Settings 
            {
                ClientId = Environment.GetEnvironmentVariable("clientId"),
                ClientSecret = Environment.GetEnvironmentVariable("clientSecret"),
                TenantId = Environment.GetEnvironmentVariable("tenantId"),
                ProjectOnlineSiteUrl = Environment.GetEnvironmentVariable("projectOnlineSiteUrl"),
                Scope = Environment.GetEnvironmentVariable("scope")
            };
        }
    }
}