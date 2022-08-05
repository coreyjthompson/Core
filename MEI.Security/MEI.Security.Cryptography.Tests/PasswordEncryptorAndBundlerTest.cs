namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class PasswordEncryptorAndBundlerTest
    {
        private PasswordEncryptorAndBundler _target;

        private Mock<ISymmetricEncryptor> _encryptor;
        private Mock<IHMACBundler> _bundler;
        private Mock<IPasswordKeyGenerator> _passwordKeyGenerator;

        private readonly Random _random = new Random();
        private readonly RNGCryptoServiceProvider _cryptoRandom = new RNGCryptoServiceProvider();

        private string _password;
        private byte[] _secretMessage;
        private byte[] _nonSecretPayload;
        private byte[] _encryptedMessage;

        [TestInitialize]
        public void Initialize()
        {
            _encryptor = new Mock<ISymmetricEncryptor>();
            _bundler = new Mock<IHMACBundler>();
            _passwordKeyGenerator = new Mock<IPasswordKeyGenerator>();

            const int keySize = 16;

            byte[] authKey = CreateBytes(keySize);
            byte[] authKeySalt = CreateBytes(8);
            byte[] cryptKey = CreateBytes(keySize);
            byte[] cryptKeySalt = CreateBytes(8);

            _secretMessage = Encoding.UTF8.GetBytes(CreateString(20));
            _nonSecretPayload = CreateBytes(16);
            _encryptedMessage = Encoding.UTF8.GetBytes(CreateString(10 + (keySize * 2) + _nonSecretPayload.Length));

            _passwordKeyGenerator.Setup(x => x.GenerateKeys(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
                new PasswordKeys
                {
                    AuthKey = authKey,
                    AuthKeySalt = authKeySalt,
                    CryptKey = cryptKey,
                    CryptKeySalt = cryptKeySalt
                });
            _passwordKeyGenerator.Setup(x =>
                x.GenerateKeys(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new PasswordKeys
                         {
                             AuthKey = authKey,
                             AuthKeySalt = authKeySalt,
                             CryptKey = cryptKey,
                             CryptKeySalt = cryptKeySalt
                         });

            _target = new PasswordEncryptorAndBundler(_encryptor.Object, _bundler.Object, _passwordKeyGenerator.Object);

            _password = CreateString(_target.MinPasswordLength);
        }

        [TestMethod]
        public void Encrypt_NullSecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt((string)null, _password, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_EmptySecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(string.Empty, _password, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_NullSecretMessage2_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt((byte[])null, _password, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_ZeroLengthSecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(new byte[] {}, _password, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_NullPassword_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(_secretMessage, null, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_EmptyPassword_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(_secretMessage, string.Empty, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_PasswordTooShort_ThrowException()
        {
            string tooShortPassword = CreateString(_target.MinPasswordLength - 1);

            Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(_secretMessage, tooShortPassword, _nonSecretPayload));
        }

        [TestMethod]
        public void Encrypt_ReturnValidEncryptedMessage()
        {
            byte[] expected = _encryptedMessage;
            var bundle = new Mock<IHMACBundle>();
            bundle.SetupGet(x => x.EncryptedMessage).Returns(expected);
            _bundler.Setup(x => x.Bundle(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(bundle.Object);

            byte[] actual = _target.Encrypt(_secretMessage, _password, _nonSecretPayload);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Encrypt2_ReturnValidEncryptedMessage()
        {
            string expected = Convert.ToBase64String(_encryptedMessage);
            var bundle = new Mock<IHMACBundle>();
            bundle.SetupGet(x => x.EncryptedMessage).Returns(_encryptedMessage);
            _bundler.Setup(x => x.Bundle(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(bundle.Object);
            string secretMessageString = Encoding.UTF8.GetString(_secretMessage);

            string actual = _target.Encrypt(secretMessageString, _password, _nonSecretPayload);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Encrypt_NullCipherText_ReturnNull()
        {
            var bundle = new Mock<IHMACBundle>();
            bundle.SetupGet(x => x.EncryptedMessage).Returns((byte[])null);
            _bundler.Setup(x => x.Bundle(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(bundle.Object);
            string secretMessageString = Encoding.UTF8.GetString(_secretMessage);

            string actual = _target.Encrypt(secretMessageString, _password, _nonSecretPayload);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Decrypt_NullEncryptedMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt((string)null, _password, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_EmptyEncryptedMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(string.Empty, _password, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_NullPlainText_ReturnNull()
        {
            string encryptedMessageString = Convert.ToBase64String(_encryptedMessage);
            var bundle = new Mock<IHMACBundle>();
            bundle.SetupGet(x => x.NonSecretPayload).Returns(_nonSecretPayload);
            bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(true);
            _bundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);
            _encryptor.Setup(x => x.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns((byte[])null);

            string actual = _target.Decrypt(encryptedMessageString, _password, _nonSecretPayload.Length);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Decrypt_InvalidBundle_ReturnNull()
        {
            var bundle = new Mock<IHMACBundle>();
            bundle.SetupGet(x => x.NonSecretPayload).Returns(_nonSecretPayload);
            bundle.Setup(x => x.IsValid(It.IsAny<byte[]>())).Returns(false);
            _bundler.Setup(x => x.UnBundle(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(bundle.Object);

            byte[] actual = _target.Decrypt(_encryptedMessage, _password, _nonSecretPayload.Length);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Decrypt2_NullEncryptedMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt((byte[])null, _password, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_ZeroLengthEncryptedMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(new byte[] {}, _password, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_NullPassword_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(_encryptedMessage, null, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_EmptyPassword_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(_encryptedMessage, string.Empty, _nonSecretPayload.Length));
        }

        [TestMethod]
        public void Decrypt_TooShortPassword_ThrowException()
        {
            string tooShortPassword = CreateString(_target.MinPasswordLength - 1);

            Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(_encryptedMessage, tooShortPassword, _nonSecretPayload.Length));
        }

        private string CreateString(int size)
        {
            const string availableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(availableCharacters, size)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
