namespace MEI.Security.AzureKeyVault
{
    using MEI.Security.Cryptography;

    public class AzureKeyVaultFactory
        : IKeyVaultFactory
    {
        public IKeyVault Create(string authClientId, string authSecret)
        {
            return new AzureKeyVault(authClientId, authSecret);
        }
    }
}
