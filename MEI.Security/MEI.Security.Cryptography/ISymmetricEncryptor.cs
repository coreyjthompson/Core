namespace MEI.Security.Cryptography
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	public interface ISymmetricEncryptor
	{
		byte[] IV { get; }

		int BlockSize { get; }

		int KeySize { get; }

		void GenerateIV();

		string Encrypt(byte[] key, byte[] iv, string secretMessage);

		byte[] Encrypt(byte[] key, byte[] iv, byte[] secretMessage);

		string Decrypt(byte[] key, byte[] iv, string secretMessage);

		byte[] Decrypt(byte[] key, byte[] iv, byte[] encryptedMessage);
	}

	public class SymmetricEncryptor
		: ISymmetricEncryptor
	{
		private readonly SymmetricAlgorithm _algorithm;

		public SymmetricEncryptor(SymmetricAlgorithm algorithm)
		{
			_algorithm = algorithm;
		}

		public byte[] IV => _algorithm.IV;

		public int BlockSize => _algorithm.BlockSize;

		public int KeySize => _algorithm.KeySize;

		public void GenerateIV()
		{
			_algorithm.GenerateIV();
		}

		public string Encrypt(byte[] key, byte[] iv, string secretMessage)
		{
			if (string.IsNullOrEmpty(secretMessage))
			{
				throw new ArgumentNullException(nameof(secretMessage));
			}

			byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
			byte[] cipherText = Encrypt(key, iv, plainText);

			return Convert.ToBase64String(cipherText);
		}

		public byte[] Encrypt(byte[] key, byte[] iv, byte[] secretMessage)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (key.Length != KeySize / 8)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize), nameof(key));
			}

            using (ICryptoTransform encryptor = _algorithm.CreateEncryptor(key, iv))
			using (var cipherStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(cipherStream, encryptor, CryptoStreamMode.Write))
				using (var binaryWriter = new BinaryWriter(cryptoStream))
				{
					binaryWriter.Write(secretMessage);
				}

				return cipherStream.ToArray();
			}
		}

		public string Decrypt(byte[] key, byte[] iv, string secretMessage)
		{
			if (string.IsNullOrEmpty(secretMessage))
			{
				throw new ArgumentNullException(nameof(secretMessage));
			}

			byte[] plainText = Decrypt(key, iv, Convert.FromBase64String(secretMessage));

			return plainText == null ? null : Encoding.UTF8.GetString(plainText);
		}

		public byte[] Decrypt(byte[] key, byte[] iv, byte[] encryptedMessage)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (key.Length != KeySize / 8)
			{
				throw new ArgumentException(string.Format(Resource1.Key_needs_to_be__0__bit, KeySize), nameof(key));
			}

            using (ICryptoTransform decryptor = _algorithm.CreateDecryptor(key, iv))
			using (var plainTextStream = new MemoryStream())
			{
				using (var decryptorStream = new CryptoStream(plainTextStream, decryptor, CryptoStreamMode.Write))
				using (var binaryWriter = new BinaryWriter(decryptorStream))
				{
					binaryWriter.Write(encryptedMessage);
				}

				return plainTextStream.ToArray();
			}
		}
	}
}
