namespace MEI.Security.Cryptography
{
	using System;
	using System.Security.Cryptography;
	using System.Text;

	using NodaTime;

	public interface IKeyVersionEncryptorAndBundler
	{
		byte[] NewKey();

		string Encrypt(string secretMessage);

		byte[] Encrypt(byte[] secretMessage);

		string Encrypt(string secretMessage, int authKeyVersionNumber, int cryptKeyVersionNumber);

		byte[] Encrypt(byte[] secretMessage, int authKeyVersionNumber, int cryptKeyVersionNumber);

		string Decrypt(string encryptedMessage, int? expirationMinutes = null);

		byte[] Decrypt(byte[] encryptedMessage, int? expirationMinutes = null);

		string Decrypt(string encryptedMessage, int nonSecretPayloadLength, int? expirationMinutes);

		byte[] Decrypt(byte[] encryptedMessage, int nonSecretPayloadLength, int? expirationMinutes);
	}

	public class KeyVersionEncryptorAndBundler
		: IKeyVersionEncryptorAndBundler
	{
		private readonly ISymmetricEncryptor _encryptor;
		private readonly IKeyVersionHMACBundler _hmacBundler;
		private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();
		private readonly IKeyManager _keyManager;
		private readonly IClock _clock;
		private readonly int _blockSize;
		private readonly int _nonSecretPayloadSize;

		public KeyVersionEncryptorAndBundler(ISymmetricEncryptor encryptor, IKeyVersionHMACBundler hmacBundler, IKeyManager keyManager, IClock clock)
		{
		    _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
			_hmacBundler = hmacBundler ?? throw new ArgumentNullException(nameof(hmacBundler));
			_keyManager = keyManager ?? throw new ArgumentNullException(nameof(keyManager));
			_clock = clock ?? throw new ArgumentNullException(nameof(clock));

			_blockSize = encryptor.BlockSize / 8;
			_nonSecretPayloadSize = 128 / 8;
		}

	    public int KeySize => _encryptor.KeySize / 8;

	    public byte[] NewKey()
		{
			var key = new byte[KeySize];
			_random.GetBytes(key);

			return key;
		}

		public string Encrypt(string secretMessage)
		{
		    if (string.IsNullOrEmpty(secretMessage))
		    {
		        throw new ArgumentNullException(nameof(secretMessage));
		    }

			return Encrypt(secretMessage, _keyManager.CurrentAuthKeyVersionNumber, _keyManager.CurrentCryptKeyVersionNumber);
		}

		public byte[] Encrypt(byte[] secretMessage)
		{
			if (secretMessage == null)
			{
				throw new ArgumentNullException(nameof(secretMessage));
			}

			return Encrypt(secretMessage, _keyManager.CurrentAuthKeyVersionNumber, _keyManager.CurrentCryptKeyVersionNumber);
		}

		public string Encrypt(string secretMessage, int authKeyVersionNumber, int cryptKeyVersionNumber)
		{
			if (string.IsNullOrEmpty(secretMessage))
			{
				throw new ArgumentNullException(nameof(secretMessage));
			}

			byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
			byte[] encryptedMessage = Encrypt(plainText, authKeyVersionNumber, cryptKeyVersionNumber);

			if (encryptedMessage == null)
			{
				return null;
			}

			return Convert.ToBase64String(encryptedMessage);
		}

		public byte[] Encrypt(byte[] secretMessage, int authKeyVersionNumber, int cryptKeyVersionNumber)
		{
		    if (secretMessage == null || secretMessage.Length < 1)
		    {
		        throw new ArgumentNullException(nameof(secretMessage));
		    }

            if (_keyManager.AuthKeys == null || _keyManager.AuthKeys.Count == 0)
			{
				throw new Exception(Resource1.KeyManager_has_zero_auth_keys);
			}

			if (_keyManager.CryptKeys == null || _keyManager.CryptKeys.Count == 0)
			{
				throw new Exception(Resource1.KeyManager_has_zero_crypt_keys);
			}

			if (!_keyManager.AuthKeys.ContainsKey(authKeyVersionNumber))
			{
				throw new Exception(string.Format(Resource1.key_with_the_specified_version_number_does_not_exist, authKeyVersionNumber));
			}

			if (!_keyManager.CryptKeys.ContainsKey(cryptKeyVersionNumber))
			{
				throw new Exception(string.Format(Resource1.key_with_the_specified_version_number_does_not_exist, cryptKeyVersionNumber));
			}

			byte[] authKey = _keyManager.AuthKeys[authKeyVersionNumber];
			byte[] cryptKey = _keyManager.CryptKeys[cryptKeyVersionNumber];

			if (cryptKey == null || cryptKey.Length != KeySize)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize * 8), nameof(cryptKey));
			}

			if (authKey == null || authKey.Length != KeySize)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize * 8), nameof(authKey));
			}

			_encryptor.GenerateIV();
			byte[] iv = _encryptor.IV;

			byte[] cipherText = _encryptor.Encrypt(cryptKey, iv, secretMessage);

			IKeyVersionHMACBundle bundle = _hmacBundler.Bundle(authKey, iv, cipherText, authKeyVersionNumber, cryptKeyVersionNumber, _clock.GetCurrentInstant());

			return bundle.EncryptedMessage;
		}

		public string Decrypt(string encryptedMessage, int? expirationMinutes = null)
		{
			return Decrypt(encryptedMessage, _nonSecretPayloadSize, expirationMinutes);
		}

		public byte[] Decrypt(byte[] encryptedMessage, int? expirationMinutes = null)
		{
			return Decrypt(encryptedMessage, _nonSecretPayloadSize, expirationMinutes);
		}

		public string Decrypt(string encryptedMessage, int nonSecretPayloadLength, int? expirationMinutes)
		{
			if (string.IsNullOrEmpty(encryptedMessage))
			{
				throw new ArgumentNullException(nameof(encryptedMessage));
			}

			byte[] plainText = Decrypt(Convert.FromBase64String(encryptedMessage), nonSecretPayloadLength, expirationMinutes);

			return plainText == null ? null : Encoding.UTF8.GetString(plainText);
		}

		public byte[] Decrypt(byte[] encryptedMessage, int nonSecretPayloadLength = 0, int? expirationMinutes = null)
		{
		    if (encryptedMessage == null || encryptedMessage.Length == 0)
		    {
		        throw new ArgumentNullException(nameof(encryptedMessage));
		    }

            IKeyVersionHMACBundle bundle = _hmacBundler.UnBundle(encryptedMessage, nonSecretPayloadLength, _blockSize);

			if (_keyManager.AuthKeys == null || _keyManager.AuthKeys.Count == 0)
			{
				throw new Exception(Resource1.KeyManager_has_zero_auth_keys);
			}

			if (_keyManager.CryptKeys == null || _keyManager.CryptKeys.Count == 0)
			{
				throw new Exception(Resource1.KeyManager_has_zero_crypt_keys);
			}

			if (!_keyManager.AuthKeys.ContainsKey(bundle.AuthKeyVersionNumber))
			{
				throw new Exception(string.Format(Resource1.key_with_the_specified_version_number_does_not_exist, bundle.AuthKeyVersionNumber));
			}

			if (!_keyManager.CryptKeys.ContainsKey(bundle.CryptKeyVersionNumber))
			{
				throw new Exception(string.Format(Resource1.key_with_the_specified_version_number_does_not_exist, bundle.CryptKeyVersionNumber));
			}

			byte[] authKey = _keyManager.AuthKeys[bundle.AuthKeyVersionNumber];
			byte[] cryptKey = _keyManager.CryptKeys[bundle.CryptKeyVersionNumber];

			if (cryptKey.Length != KeySize)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize * 8), nameof(cryptKey));
			}

			if (authKey.Length != KeySize)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize * 8), nameof(authKey));
			}

			if (!bundle.IsValid(authKey))
			{
				// QUESTION: what to do for bad hmac?
				return null;
			}

			if (expirationMinutes != null)
			{
				Instant expirationInstant = bundle.EncryptionInstant.Plus(Duration.FromMinutes((long)expirationMinutes));

				if (_clock.GetCurrentInstant() > expirationInstant)
				{
					return null;
				}
			}

			return _encryptor.Decrypt(cryptKey, bundle.IV, bundle.CipherText);
		}
    }
}
