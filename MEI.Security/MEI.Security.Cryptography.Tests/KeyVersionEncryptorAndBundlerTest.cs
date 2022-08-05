namespace MEI.Security.Cryptography.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using NodaTime;

	[TestClass]
	public class KeyVersionEncryptorAndBundlerTest
	{
		private KeyVersionEncryptorAndBundler _target;

		private Mock<IKeyManager> _keyManager;
		private Mock<IClock> _clock;
		private Mock<ISymmetricEncryptor> _encryptor;
		private Mock<IKeyVersionHMACBundler> _hmacBundler;
		private Mock<IHMACFactory> _hmacFactory;

		private readonly RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

		private const int currentAuthKeyVersionNumber = 2;
		private const int currentCryptKeyVersionNumber = 2;
	    private const string decryptedText = "something";
	    private const string encryptedMessage = "c29tZXRoaW5n";
	    private const int nonSecretPayloadLength = 16;
	    private const int validAuthKeyVersionNumber = 2;
	    private const int nullAuthKeyVersionNumber = 3;
	    private const int invalidLengthAuthKeyVersionNumber = 4;
	    private const int nonExistentAuthKeyVersionNumber = 1;
	    private const int validCryptKeyVersionNumber = 2;
	    private const int nullCryptKeyVersionNumber = 3;
	    private const int invalidLengthCryptKeyVersionNumber = 4;
	    private const int nonExistentCryptKeyVersionNumber = 1;
	    private const int keySizeInBits = 256;
	    private const int blockSizeInBits = 128;
	    private const int ivSizeInBytes = 16;

		private byte[] _authKey;
		private byte[] _cryptKey;

		[TestInitialize]
		public void Initialize()
		{
			_keyManager = new Mock<IKeyManager>();
			_clock = new Mock<IClock>();
			_encryptor = new Mock<ISymmetricEncryptor>();
			_hmacBundler = new Mock<IKeyVersionHMACBundler>();
			_hmacFactory = new Mock<IHMACFactory>();

			_authKey = CreateBytes(keySizeInBits / 8);
			_cryptKey = CreateBytes(keySizeInBits / 8);

			byte[] iv = CreateBytes(ivSizeInBytes);

			_encryptor.SetupGet(x => x.IV).Returns(iv);
			_encryptor.SetupGet(x => x.KeySize).Returns(keySizeInBits);
		    _encryptor.SetupGet(x => x.BlockSize).Returns(blockSizeInBits);

			_keyManager.SetupGet(x => x.AuthKeys).Returns(
				new Dictionary<int, byte[]>
				{
					{ validAuthKeyVersionNumber, _authKey },
					{ nullAuthKeyVersionNumber, null },
					{ invalidLengthAuthKeyVersionNumber, new byte[(keySizeInBits / 8) - 1] }
				});
			_keyManager.SetupGet(x => x.CryptKeys).Returns(
				new Dictionary<int, byte[]>
				{
					{ validCryptKeyVersionNumber, _cryptKey },
					{ nullCryptKeyVersionNumber, null },
					{ invalidLengthCryptKeyVersionNumber, new byte[(keySizeInBits / 8) - 1] }
				});
			_keyManager.SetupGet(x => x.CurrentAuthKeyVersionNumber).Returns(currentAuthKeyVersionNumber);
			_keyManager.SetupGet(x => x.CurrentCryptKeyVersionNumber).Returns(currentCryptKeyVersionNumber);

			_target = new KeyVersionEncryptorAndBundler(_encryptor.Object, _hmacBundler.Object, _keyManager.Object, _clock.Object);
		}

	    [TestMethod]
	    public void NewKey_AppropriateSize()
	    {
	        byte[] value = _target.NewKey();

            Assert.AreEqual(_target.KeySize, value.Length);
	    }

		[TestMethod]
		public void Encrypt_EmptySecretMessage_ThrowException()
		{
			Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(string.Empty));
		}

	    [TestMethod]
	    public void Encrypt_NullSecretMessage_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt((string)null));
	    }

	    [TestMethod]
	    public void Encrypt_EmptySecretMessage2_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() =>
	            _target.Encrypt(string.Empty, currentAuthKeyVersionNumber, currentCryptKeyVersionNumber));
	    }

	    [TestMethod]
	    public void Encrypt_NullSecretMessage2_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() =>
	            _target.Encrypt((string)null, currentAuthKeyVersionNumber, currentCryptKeyVersionNumber));
	    }

	    [TestMethod]
	    public void Encrypt_NullSecretMessage3_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() =>
	            _target.Encrypt((byte[])null, currentAuthKeyVersionNumber, currentCryptKeyVersionNumber));
	    }

        [TestMethod]
	    public void Encrypt_ZeroLengthSecretMessage_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() =>
	            _target.Encrypt(new byte[] { }, currentAuthKeyVersionNumber, currentCryptKeyVersionNumber));
	    }

		[TestMethod]
		public void Encrypt_NullEncryptionResult_ReturnNull()
		{
			_hmacBundler.Setup(
					x => x.Bundle(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Instant>()))
				.Returns(new KeyVersionHMACBundle(_hmacFactory.Object, null, null, null, null, 0, 0, Instant.MaxValue, null));

			string result = _target.Encrypt(decryptedText);

			Assert.IsNull(result);
		}

		[TestMethod]
		public void Encrypt_ZeroAuthKeys_ThrowException()
		{
			_keyManager.SetupGet(x => x.AuthKeys).Returns(new Dictionary<int, byte[]>());

			Assert.ThrowsException<Exception>(() => _target.Encrypt(decryptedText));
		}

		[TestMethod]
		public void Encrypt_ZeroCryptKeys_ThrowException()
		{
			_keyManager.SetupGet(x => x.CryptKeys).Returns(new Dictionary<int, byte[]>());

			Assert.ThrowsException<Exception>(() => _target.Encrypt(decryptedText));
		}

		[TestMethod]
		public void Encrypt_NonExistentAuthKeyVersionNumber_ThrowException()
		{
			Assert.ThrowsException<Exception>(() => _target.Encrypt(decryptedText, nonExistentAuthKeyVersionNumber, validCryptKeyVersionNumber));
		}

		[TestMethod]
		public void Encrypt_NonExistentCryptKeyVersionNumber_ThrowException()
		{
			Assert.ThrowsException<Exception>(() => _target.Encrypt(decryptedText, validAuthKeyVersionNumber, nonExistentCryptKeyVersionNumber));
		}

		[TestMethod]
		public void Encrypt_NoAuthKeyVersionNumberSupplied_UseCurrentAuthKeyVersionNumber()
		{
			_hmacBundler
				.Setup(x => x.Bundle(It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<Instant>()))
				.Returns(new KeyVersionHMACBundle(_hmacFactory.Object, null, null, null, null, 0, 0, Instant.MaxValue, null));

			_target.Encrypt(decryptedText);

			_hmacBundler.Verify(x => x.Bundle(_authKey,
					It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					currentAuthKeyVersionNumber,
					It.IsAny<int>(),
					It.IsAny<Instant>()),
				Times.AtLeastOnce);
		}

		[TestMethod]
		public void Encrypt_NoCryptKeyVersionNumberSupplied_UseCurrentCryptKeyVersionNumber()
		{
			_hmacBundler
				.Setup(x => x.Bundle(It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<Instant>())).Returns(new KeyVersionHMACBundle(_hmacFactory.Object,
					CreateBytes(5),
					null,
					null,
					null,
					0,
					0,
					Instant.MaxValue,
					null));

			_target.Encrypt(decryptedText);

			_hmacBundler.Verify(x => x.Bundle(_authKey,
					It.IsAny<byte[]>(),
					It.IsAny<byte[]>(),
					It.IsAny<int>(),
					currentCryptKeyVersionNumber,
					It.IsAny<Instant>()),
				Times.AtLeastOnce);
		}

		[TestMethod]
		public void Encrypt_NullCryptKey_ThrowException()
		{
			Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(decryptedText, validAuthKeyVersionNumber, nullCryptKeyVersionNumber));
		}

		[TestMethod]
		public void Encrypt_NullAuthKey_ThrowException()
		{
			Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(decryptedText, nullAuthKeyVersionNumber, validCryptKeyVersionNumber));
		}

		[TestMethod]
		public void Encrypt_WrongCryptKeySize_ThrowException()
		{
			Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(decryptedText, validAuthKeyVersionNumber, invalidLengthCryptKeyVersionNumber));
		}

		[TestMethod]
		public void Encrypt_WrongAuthKeySize_ThrowException()
		{
			Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(decryptedText, invalidLengthAuthKeyVersionNumber, validCryptKeyVersionNumber));
		}

        [TestMethod]
	    public void Encrypt_ReturnValidEncryptedMessage()
        {
            byte[] expected = CreateBytes(10);
	        _hmacBundler.Setup(x => x.Bundle(It.IsAny<byte[]>(),
		        It.IsAny<byte[]>(),
		        It.IsAny<byte[]>(),
		        It.IsAny<int>(),
		        It.IsAny<int>(),
		        It.IsAny<Instant>())).Returns(new KeyVersionHMACBundle(_hmacFactory.Object,
		        expected,
		        null,
		        null,
		        null,
		        currentAuthKeyVersionNumber,
		        currentCryptKeyVersionNumber,
		        Instant.MaxValue,
		        null));

            string actual = _target.Encrypt(decryptedText);

            Assert.AreEqual(Convert.ToBase64String(expected), actual);
        }

	    [TestMethod]
	    public void Encrypt_NullSecretMessageBytes_ThrowException()
	    {
		    Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt((byte[])null));
	    }

	    [TestMethod]
	    public void Encrypt_ValidSecretMessageBytes_ReturnValidEncryptedMessage()
	    {
		    byte[] expected = CreateBytes(10);
		    _hmacBundler.Setup(x => x.Bundle(It.IsAny<byte[]>(),
			    It.IsAny<byte[]>(),
			    It.IsAny<byte[]>(),
			    It.IsAny<int>(),
			    It.IsAny<int>(),
			    It.IsAny<Instant>())).Returns(new KeyVersionHMACBundle(_hmacFactory.Object,
			    expected,
			    null,
			    null,
			    null,
			    currentAuthKeyVersionNumber,
			    currentCryptKeyVersionNumber,
			    Instant.MaxValue,
			    null));

		    byte[] actual = _target.Encrypt(Encoding.UTF8.GetBytes(decryptedText));

			Assert.IsTrue(expected.SequenceEqual(actual));
	    }

	    [TestMethod]
	    public void Decrypt_NullEncryptedMessage_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt((string)null));
	    }

	    [TestMethod]
	    public void Decrypt_EmptyEncryptedMessage_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(string.Empty));
	    }

	    [TestMethod]
	    public void Decrypt_NullEncryptedMessage2_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt((string)null, nonSecretPayloadLength));
	    }

	    [TestMethod]
	    public void Decrypt_EmptyEncryptedMessage2_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(string.Empty, nonSecretPayloadLength));
	    }

	    [TestMethod]
	    public void Decrypt_ZeroAuthKeys_ThrowException()
	    {
	        _keyManager.SetupGet(x => x.AuthKeys).Returns(new Dictionary<int, byte[]>());

	        Assert.ThrowsException<Exception>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_EmptyCryptKeys_ThrowException()
	    {
	        _keyManager.SetupGet(x => x.CryptKeys).Returns(new Dictionary<int, byte[]>());

	        Assert.ThrowsException<Exception>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_NonExistentAuthKeyVersionNumber_ThrowException()
	    {
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
			    new KeyVersionHMACBundle(_hmacFactory.Object,
				    null,
				    null,
				    null,
				    null,
				    nonExistentAuthKeyVersionNumber,
				    validCryptKeyVersionNumber,
				    Instant.MaxValue,
				    null));

            Assert.ThrowsException<Exception>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_NonExistentCryptKeyVersionNumber_ThrowException()
	    {
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
			    new KeyVersionHMACBundle(_hmacFactory.Object,
				    null,
				    null,
				    null,
				    null,
				    validAuthKeyVersionNumber,
				    nonExistentCryptKeyVersionNumber,
				    Instant.MaxValue,
				    null));

	        Assert.ThrowsException<Exception>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_WrongCryptKeySize_ThrowException()
	    {
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
			    new KeyVersionHMACBundle(_hmacFactory.Object,
				    null,
				    null,
				    null,
				    null,
				    validAuthKeyVersionNumber,
				    invalidLengthCryptKeyVersionNumber,
				    Instant.MaxValue,
				    null));

	        Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_WrongAuthKeySize_ThrowException()
	    {
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
			    new KeyVersionHMACBundle(_hmacFactory.Object,
				    null,
				    null,
				    null,
				    null,
				    invalidLengthAuthKeyVersionNumber,
				    validCryptKeyVersionNumber,
				    Instant.MaxValue,
				    null));

	        Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(encryptedMessage));
	    }

	    [TestMethod]
	    public void Decrypt_NullEncryptedMessage3_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt((byte[])null, nonSecretPayloadLength));
	    }

	    [TestMethod]
	    public void Decrypt_ZeroLengthEncryptedMessage_ThrowException()
	    {
	        Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(new byte[] {}, nonSecretPayloadLength));
	    }

	    [TestMethod]
	    public void Decrypt_InvalidBundle_ReturnNull()
	    {
	        var bundle = new Mock<IKeyVersionHMACBundle>();
	        bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(false);
	        bundle.SetupGet(x => x.AuthKeyVersionNumber).Returns(validAuthKeyVersionNumber);
	        bundle.SetupGet(x => x.CryptKeyVersionNumber).Returns(validCryptKeyVersionNumber);
	        _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);

	        string actual = _target.Decrypt(encryptedMessage);

            Assert.IsNull(actual);
	    }

	    [TestMethod]
	    public void Decrypt_ReturnValidDecryptedMessage()
	    {
	        string expected = decryptedText;
	        var bundle = new Mock<IKeyVersionHMACBundle>();
	        bundle.SetupGet(x => x.AuthKeyVersionNumber).Returns(currentAuthKeyVersionNumber);
	        bundle.SetupGet(x => x.CryptKeyVersionNumber).Returns(currentCryptKeyVersionNumber);
	        bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(true);
	        _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);
	        _encryptor.Setup(x => x.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
	            .Returns(Encoding.UTF8.GetBytes(expected));

	        string actual = _target.Decrypt(encryptedMessage);

            Assert.AreEqual(expected, actual);
	    }

	    [TestMethod]
	    public void Decrypt_ValidEncryptedMessageMessageBytes_ReturnValidDecryptedMessage()
	    {
		    byte[] expected = Encoding.UTF8.GetBytes(decryptedText);
		    var bundle = new Mock<IKeyVersionHMACBundle>();
		    bundle.SetupGet(x => x.AuthKeyVersionNumber).Returns(currentAuthKeyVersionNumber);
		    bundle.SetupGet(x => x.CryptKeyVersionNumber).Returns(currentCryptKeyVersionNumber);
		    bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(true);
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);
		    _encryptor.Setup(x => x.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
			    .Returns(Encoding.UTF8.GetBytes(decryptedText));

		    byte[] actual = _target.Decrypt(Encoding.UTF8.GetBytes(encryptedMessage), null);

			Assert.IsTrue(expected.SequenceEqual(actual));
	    }

	    [TestMethod]
	    public void Decrypt_HasExpirationMinutesAndIsWithinThem_ReturnValidDecryptedMessage()
	    {
		    byte[] expected = Encoding.UTF8.GetBytes(decryptedText);
		    var bundle = new Mock<IKeyVersionHMACBundle>();
		    Instant thisInstant = Instant.FromDateTimeOffset(new DateTimeOffset(2019, 5, 10, 10, 2, 0, TimeSpan.FromHours(-5)));
		    _clock.Setup(x => x.GetCurrentInstant()).Returns(thisInstant);
		    bundle.SetupGet(x => x.AuthKeyVersionNumber).Returns(currentAuthKeyVersionNumber);
		    bundle.SetupGet(x => x.CryptKeyVersionNumber).Returns(currentCryptKeyVersionNumber);
		    var expirationMinutes = 10;
		    bundle.SetupGet(x => x.EncryptionInstant).Returns(thisInstant.Minus(Duration.FromMinutes(expirationMinutes - 1)));
		    bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(true);
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);
		    _encryptor.Setup(x => x.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(expected);

		    byte[] actual = _target.Decrypt(Encoding.UTF8.GetBytes(encryptedMessage), nonSecretPayloadLength, expirationMinutes);

		    Assert.IsTrue(expected.SequenceEqual(actual));
	    }

	    [TestMethod]
	    public void Decrypt_HasExpirationMinutesAndIsOutsideOfThem_ThrowException()
	    {
		    var bundle = new Mock<IKeyVersionHMACBundle>();
		    Instant thisInstant = Instant.FromDateTimeOffset(new DateTimeOffset(2019, 5, 10, 10, 2, 0, TimeSpan.FromHours(-5)));
		    _clock.Setup(x => x.GetCurrentInstant()).Returns(thisInstant);
		    bundle.SetupGet(x => x.AuthKeyVersionNumber).Returns(currentAuthKeyVersionNumber);
		    bundle.SetupGet(x => x.CryptKeyVersionNumber).Returns(currentCryptKeyVersionNumber);
		    var expirationMinutes = 10;
		    bundle.SetupGet(x => x.EncryptionInstant).Returns(thisInstant.Minus(Duration.FromMinutes(expirationMinutes + 1)));
		    bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(true);
		    _hmacBundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);

		    _target.Decrypt(Encoding.UTF8.GetBytes(encryptedMessage), nonSecretPayloadLength, expirationMinutes);
	    }

		private byte[] CreateBytes(int size)
		{
			var buff = new byte[size];
			_random.GetBytes(buff);

			return buff;
		}
	}
}
