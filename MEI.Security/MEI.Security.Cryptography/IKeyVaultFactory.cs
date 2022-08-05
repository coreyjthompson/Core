namespace MEI.Security.Cryptography
{
    public interface IKeyVaultFactory
    {
        IKeyVault Create(string authClientId, string authSecret);
    }
}
