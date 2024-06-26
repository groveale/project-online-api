using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class KeyVaultHelper
{
    private SecretClient _client;

    public KeyVaultHelper(string KeyVaultName, string clientId, string clientSecret, string tenantId)
    {
        // for when we have no managed identity i.e. local dev
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var vaultUri = $"https://{KeyVaultName}.vault.azure.net/";
        _client = new SecretClient(new Uri(vaultUri), credential);
    }

    public KeyVaultHelper(string KeyVaultName)
    {
        // For Azure
        var credential = new DefaultAzureCredential();
        var vaultUri = $"https://{KeyVaultName}.vault.azure.net/";
        _client = new SecretClient(new Uri(vaultUri), credential);
    }

    public string GetSecret(string secretName)
    {
        try {
            KeyVaultSecret secret = _client.GetSecret(secretName);
            Console.WriteLine($"{secretName} obtained.");
            return secret.Value;
        }
        catch {
            // secret does not exist
            return "";
        }
        
    }

    public void SetSecret(string secretName, string secretValue)
    {
        _client.SetSecret(secretName, secretValue);
        Console.WriteLine($"{secretName} updated.");
    }

    public KeyVaultSecret GetSecretDetails(string secretName)
    {
        KeyVaultSecret secret = _client.GetSecret(secretName);
        Console.WriteLine($"{secretName} obtained.");
        return secret;
    }
}