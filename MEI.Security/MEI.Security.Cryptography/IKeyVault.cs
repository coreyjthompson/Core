namespace MEI.Security.Cryptography
{
    using System;

    public interface IKeyVault
        : IDisposable
    {
        string GetSecretById(string id);
    }
}
