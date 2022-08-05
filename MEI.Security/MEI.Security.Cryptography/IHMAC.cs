namespace MEI.Security.Cryptography
{
    using System;

    public interface IHMAC
        : IDisposable
    {
        int HashSize { get; }

        byte[] ComputeHash(byte[] buffer);

        byte[] ComputeHash(byte[] buffer, int offset, int count);
    }
}
