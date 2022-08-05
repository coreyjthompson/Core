namespace MEI.Security.Cryptography
{
    using System.Security.Cryptography;

    public class HMACWrapper
        : IHMAC
    {
        private readonly HMAC _target;

        public HMACWrapper(HMAC target)
        {
            _target = target;
        }

        public int HashSize => _target.HashSize;

        public byte[] ComputeHash(byte[] buffer)
        {
            return _target.ComputeHash(buffer);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            return _target.ComputeHash(buffer, offset, count);
        }

        public void Dispose()
        {
            _target?.Dispose();
        }
    }
}
