namespace MEI.Security.Cryptography
{
    using NodaTime;

    public interface IKeyVersionHMACBundle
        : IHMACBundle
    {
        int AuthKeyVersionNumber { get; }
        int CryptKeyVersionNumber { get; }
        Instant EncryptionInstant { get; }
    }

    public class KeyVersionHMACBundle
        : HMACBundle, IKeyVersionHMACBundle
    {
        public KeyVersionHMACBundle(IHMACFactory hmacFactory,
                                    IHMACBundle bundle,
                                    int authKeyVersionNumber,
                                    int cryptKeyVersionNumber,
                                    Instant encryptionInstant)
            : base(hmacFactory, bundle.EncryptedMessage, bundle.IV, bundle.NonSecretPayload, bundle.CipherText, bundle.SentTag)
        {
            AuthKeyVersionNumber = authKeyVersionNumber;
            CryptKeyVersionNumber = cryptKeyVersionNumber;
            EncryptionInstant = encryptionInstant;
        }

        public KeyVersionHMACBundle(IHMACFactory hmacFactory,
                                    byte[] encryptedMessage,
                                    byte[] iv,
                                    byte[] nonSecretPayload,
                                    byte[] cipherText,
                                    int authKeyVersionNumber,
                                    int cryptKeyVersionNumber,
                                    Instant encryptionInstant,
                                    byte[] sentTag)
            : base(hmacFactory, encryptedMessage, iv, nonSecretPayload, cipherText, sentTag)
        {
            AuthKeyVersionNumber = authKeyVersionNumber;
            CryptKeyVersionNumber = cryptKeyVersionNumber;
            EncryptionInstant = encryptionInstant;
        }

        public int AuthKeyVersionNumber { get; }

        public int CryptKeyVersionNumber { get; }

        public Instant EncryptionInstant { get; }
    }
}
