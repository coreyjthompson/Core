namespace MEI.Security.Cryptography
{
    public interface IHMACBundle
    {
        byte[] IV { get; }
        byte[] NonSecretPayload { get; }
        byte[] CipherText { get; set; }
        byte[] EncryptedMessage { get; }
        byte[] SentTag { get; }

        bool IsValid(byte[] key);
    }

    public class HMACBundle
        : IHMACBundle
    {
        private readonly IHMACFactory _hmacFactory;

        public HMACBundle(IHMACFactory hmacFactory, byte[] encryptedMessage, byte[] iv, byte[] nonSecretPayload, byte[] cipherText, byte[] sentTag)
        {
            _hmacFactory = hmacFactory;

            EncryptedMessage = encryptedMessage;
            IV = iv;
            NonSecretPayload = nonSecretPayload;
            CipherText = cipherText;
            SentTag = sentTag;
        }

        public byte[] IV { get; }

        public byte[] NonSecretPayload { get; }

        public byte[] CipherText { get; set; }

        public byte[] EncryptedMessage { get; }

        public byte[] SentTag { get; }

        public virtual bool IsValid(byte[] key)
        {
            using (IHMAC hmac = _hmacFactory.Create("SHA256", key))
            {
                byte[] computedTag = hmac.ComputeHash(EncryptedMessage, 0, EncryptedMessage.Length - SentTag.Length);

                if (EncryptedMessage.Length < SentTag.Length + NonSecretPayload.Length + IV.Length)
                {
                    return false;
                }

                var compare = 0;
                for (var i = 0; i < SentTag.Length; i++)
                {
                    compare |= SentTag[i] ^ computedTag[i];
                }

                return compare == 0;
            }
        }
    }
}
