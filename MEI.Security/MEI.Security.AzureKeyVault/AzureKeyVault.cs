namespace MEI.Security.AzureKeyVault
{
    using System.Threading.Tasks;

    using MEI.Security.Cryptography;

    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class AzureKeyVault
        : IKeyVault
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public AzureKeyVault(string authClientId, string authSecret)
        {
            var credential = new ClientCredential(authClientId, authSecret);

            _keyVaultClient = new KeyVaultClient(
                (authority, resource, scope) => GetAccessToken(authority, resource, scope, credential),
                new InjectHostHeaderHttpMessageHandler());
        }

        public string GetSecretById(string id)
        {
            return Task.Run(() => _keyVaultClient.GetSecretAsync(id)).ConfigureAwait(false).GetAwaiter().GetResult().Value;
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope, ClientCredential credential)
        {
            var context = new AuthenticationContext(authority, null);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, credential).ConfigureAwait(false);

            return result.AccessToken;
        }

        /*private async Task<string> GetAccessToken(
            string authority,
            string resource,
            string scope,
            IClientAssertionCertificate assertionCertificate)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, assertionCertificate).ConfigureAwait(false);

            return result.AccessToken;
        }*/

        public void Dispose()
        {
            _keyVaultClient?.Dispose();
        }
    }
}
