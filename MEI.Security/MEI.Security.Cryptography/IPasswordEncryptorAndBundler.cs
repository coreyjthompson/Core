namespace MEI.Security.Cryptography
{
	using System;
	using System.Text;

	public interface IPasswordEncryptorAndBundler
	{
		string Encrypt(string secretMessage, string password, byte[] nonSecretPayload = null);

		byte[] Encrypt(byte[] secretMessage, string password, byte[] nonSecretPayload = null);

		string Decrypt(string encryptedMessage, string password, int nonSecretPayloadLength = 0);

		byte[] Decrypt(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0);
	}

	public class PasswordEncryptorAndBundler
		: IPasswordEncryptorAndBundler
	{
		private readonly ISymmetricEncryptor _encryptor;
		private readonly IHMACBundler _hmacBundler;
		private readonly int _keySize;
	    private readonly IPasswordKeyGenerator _passwordKeyGenerator;

		public PasswordEncryptorAndBundler(ISymmetricEncryptor encryptor, IHMACBundler hmacBundler, IPasswordKeyGenerator passwordKeyGenerator)
		{
			_encryptor = encryptor;
			_hmacBundler = hmacBundler;
		    _passwordKeyGenerator = passwordKeyGenerator;

			_keySize = encryptor.KeySize / 8;
		}

		public int MinPasswordLength { get; } = 12;

		public int SaltSize { get; } = 64 / 8;

		public int Iterations { get; } = 10000;

		public string Encrypt(string secretMessage, string password, byte[] nonSecretPayload = null)
		{
			if (string.IsNullOrEmpty(secretMessage))
			{
				throw new ArgumentNullException(nameof(secretMessage));
			}

			byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
			byte[] cipherText = Encrypt(plainText, password, nonSecretPayload);

			return cipherText == null ? null : Convert.ToBase64String(cipherText);
		}

		public byte[] Encrypt(byte[] secretMessage, string password, byte[] nonSecretPayload = null)
		{
		    if (secretMessage == null || secretMessage.Length == 0)
		    {
		        throw new ArgumentNullException(nameof(secretMessage));
		    }

            if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
			{
				throw new ArgumentException(string.Format(Resource1.Must_have_a_password_of_at_least__0__characters, MinPasswordLength), nameof(password));
			}

		    nonSecretPayload = nonSecretPayload ?? new byte[] { };

            var payload = new byte[(SaltSize * 2) + nonSecretPayload.Length];

			Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
			int payloadIndex = nonSecretPayload.Length;

		    PasswordKeys keys = _passwordKeyGenerator.GenerateKeys(password, SaltSize, Iterations, _keySize);

		    Array.Copy(keys.CryptKeySalt, 0, payload, payloadIndex, keys.CryptKeySalt.Length);
		    payloadIndex += keys.CryptKeySalt.Length;

		    Array.Copy(keys.AuthKeySalt, 0, payload, payloadIndex, keys.AuthKeySalt.Length);

            _encryptor.GenerateIV();
			byte[] iv = _encryptor.IV;

			byte[] cipherText = _encryptor.Encrypt(keys.CryptKey, iv, secretMessage);

			IHMACBundle bundle = _hmacBundler.Bundle(keys.AuthKey, iv, cipherText, payload);

			return bundle.EncryptedMessage;
		}

		public string Decrypt(string encryptedMessage, string password, int nonSecretPayloadLength = 0)
		{
			if (string.IsNullOrEmpty(encryptedMessage))
			{
				throw new ArgumentNullException(nameof(encryptedMessage));
			}

			byte[] plainText = Decrypt(Convert.FromBase64String(encryptedMessage), password, nonSecretPayloadLength);

			return plainText == null ? null : Encoding.UTF8.GetString(plainText);
		}

		public byte[] Decrypt(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
		{
			if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
			{
				throw new ArgumentException(string.Format(Resource1.Must_have_a_password_of_at_least__0__characters, MinPasswordLength), nameof(password));
			}

			if (encryptedMessage == null || encryptedMessage.Length == 0)
			{
				throw new ArgumentNullException(nameof(encryptedMessage));
			}

			var cryptSalt = new byte[SaltSize];
			var authSalt = new byte[SaltSize];

			Array.Copy(encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
			Array.Copy(encryptedMessage, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

		    PasswordKeys keys = _passwordKeyGenerator.GenerateKeys(password, cryptSalt, authSalt, Iterations, _keySize);

            IHMACBundle bundle = _hmacBundler.UnBundle(encryptedMessage, nonSecretPayloadLength + (SaltSize * 2), _encryptor.BlockSize / 8);

			var nonSecretPayload = new byte[nonSecretPayloadLength];
			Array.Copy(bundle.NonSecretPayload, 0, nonSecretPayload, 0, nonSecretPayloadLength);

			if (!bundle.IsValid(keys.AuthKey))
			{
				// QUESTION: what to do for bad hmac?
				return null;
			}

			return _encryptor.Decrypt(keys.CryptKey, bundle.IV, bundle.CipherText);
		}
	}
}
