namespace MEI.Security.Cryptography
{
    using System;
    using System.IO;

    public interface IHMACBundler
	{
        int TagLength { get; }

		IHMACBundle Bundle(byte[] key, byte[] iv, byte[] cipherText, byte[] nonSecretPayload);

		IHMACBundle UnBundle(byte[] encryptedMessage, int nonSecretPayloadLength, int ivLength);
	}

	public class HMACBundler
        : IHMACBundler
    {
        private const string hmacKind = "SHA256";
        protected readonly IHMACFactory HMACFactory;

        public HMACBundler(IHMACFactory hmacFactory)
        {
            HMACFactory = hmacFactory;

            TagLength = hmacFactory.Create(hmacKind).HashSize / 8;
        }

        public int TagLength { get; }

        public IHMACBundle Bundle(byte[] key, byte[] iv, byte[] cipherText, byte[] nonSecretPayload)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (cipherText == null)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            // Assemble encrypted message and add authentication
            using (var hmac = HMACFactory.Create(hmacKind, key))
            using (var encryptedStream = new MemoryStream())
            {
                byte[] tag;
                using (var binaryWriter = new BinaryWriter(encryptedStream))
                {
                    // Prepend non-secret payload if any
                    if (nonSecretPayload != null)
                    {
                        binaryWriter.Write(nonSecretPayload);
                    }

                    // Prepend IV
                    binaryWriter.Write(iv);

                    // Write Ciphertext
                    binaryWriter.Write(cipherText);
                    binaryWriter.Flush();

                    // Authenticate all data
                    tag = hmac.ComputeHash(encryptedStream.ToArray());

                    // Postpend tag
                    binaryWriter.Write(tag);
                }

                return new HMACBundle(HMACFactory, encryptedStream.ToArray(), iv, nonSecretPayload, cipherText, tag);
            }
        }

        public IHMACBundle UnBundle(byte[] encryptedMessage, int nonSecretPayloadLength, int ivLength)
        {
            if (encryptedMessage == null)
            {
                throw new ArgumentNullException(nameof(encryptedMessage));
            }

            if (nonSecretPayloadLength < 0)
            {
                throw new ArgumentException(Resource1.NonSecretPayload_length_must_be_greater_than_zero);
            }

            if (ivLength < 0)
            {
                throw new ArgumentException(Resource1.IV_length_must_be_greater_than_zero);
            }

            if (encryptedMessage.Length < nonSecretPayloadLength + ivLength + TagLength)
            {
                throw new ArgumentException(Resource1.EncryptedMessage_is_not_large_enough);
            }

            var nonSecretPayload = new byte[nonSecretPayloadLength];
            Array.Copy(encryptedMessage, 0, nonSecretPayload, 0, nonSecretPayloadLength);

            var iv = new byte[ivLength];
            Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

            int cipherTextLength = encryptedMessage.Length - nonSecretPayloadLength - iv.Length - TagLength;
            var cipherText = new byte[cipherTextLength];
            Array.Copy(encryptedMessage, nonSecretPayloadLength + ivLength, cipherText, 0, cipherTextLength);

            var sentTag = new byte[TagLength];
            Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

            return new HMACBundle(HMACFactory, encryptedMessage, iv, nonSecretPayload, cipherText, sentTag);
        }
    }
}
