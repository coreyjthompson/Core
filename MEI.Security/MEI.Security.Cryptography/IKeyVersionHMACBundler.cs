namespace MEI.Security.Cryptography
{
    using System;

    using NodaTime;

    public interface IKeyVersionHMACBundler
    {
        IKeyVersionHMACBundle Bundle(
            byte[] key,
            byte[] iv,
            byte[] cipherText,
            int authKeyVersionNumber,
            int cryptKeyVersionNumber,
            Instant encryptionInstant);

        IKeyVersionHMACBundle UnBundle(byte[] encryptedMessage, int nonSecretPayloadLength, int ivLength);
    }

    public class KeyVersionHMACBundler
        : HMACBundler, IKeyVersionHMACBundler
    {
        public int KeyVersionsSize { get; } = 128 / 8;

        public int KeyVersionSize { get; } = 32 / 8;

        public int EncryptionDateSize { get; } = 64 / 8;

        public KeyVersionHMACBundler(IHMACFactory hmacFactory)
            : base(hmacFactory)
        { }

        public IKeyVersionHMACBundle Bundle(byte[] key, byte[] iv, byte[] cipherText, int authKeyVersionNumber, int cryptKeyVersionNumber, Instant encryptionInstant)
        {
            var nonSecretPayload = new byte[KeyVersionsSize];

            // auth key version number
            Array.Copy(BitConverter.GetBytes(authKeyVersionNumber), nonSecretPayload, KeyVersionSize);

            // crypt key version number
            Array.Copy(BitConverter.GetBytes(cryptKeyVersionNumber), 0, nonSecretPayload, KeyVersionSize, KeyVersionSize);

            // encryption instant
            Array.Copy(
                BitConverter.GetBytes(encryptionInstant.ToUnixTimeTicks()),
                0,
                nonSecretPayload,
                KeyVersionSize * 2,
                EncryptionDateSize);

            IHMACBundle bundle = base.Bundle(key, iv, cipherText, nonSecretPayload);

            return new KeyVersionHMACBundle(HMACFactory, bundle, authKeyVersionNumber, cryptKeyVersionNumber, encryptionInstant);
        }

        public new IKeyVersionHMACBundle UnBundle(byte[] encryptedMessage, int nonSecretPayloadLength, int ivLength)
        {
            IHMACBundle bundle = base.UnBundle(encryptedMessage, nonSecretPayloadLength, ivLength);

            var keyVersions = new byte[KeyVersionsSize];
            Array.Copy(bundle.NonSecretPayload, 0, keyVersions, 0, KeyVersionsSize);

            int authKeyVersionNumber = BitConverter.ToInt32(keyVersions, 0);
            int cryptKeyVersionNumber = BitConverter.ToInt32(keyVersions, KeyVersionSize);
            long unixTicks = BitConverter.ToInt64(keyVersions, KeyVersionSize * 2);
            Instant encryptedDate = Instant.FromUnixTimeTicks(unixTicks);

            return new KeyVersionHMACBundle(HMACFactory, bundle, authKeyVersionNumber, cryptKeyVersionNumber, encryptedDate);
        }
    }
}
