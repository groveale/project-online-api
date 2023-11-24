using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class KeyVaultHelper
{
    private SecretClient _client;

    public KeyVaultHelper(string KeyVaultName, string clientId, string clientSecret, string tenantId)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var vaultUri = $"https://{KeyVaultName}.vault.azure.net/";
        _client = new SecretClient(new Uri(vaultUri), credential);
    }

    public string GetSecret(string secretName)
    {
        KeyVaultSecret secret = _client.GetSecret(secretName);
        Console.WriteLine($"{secretName} obtained.");
        return secret.Value;
    }

    public void SetSecret(string secretName, string secretValue)
    {
        _client.SetSecret(secretName, secretValue);
        Console.WriteLine($"{secretName} updated.");
    }
}