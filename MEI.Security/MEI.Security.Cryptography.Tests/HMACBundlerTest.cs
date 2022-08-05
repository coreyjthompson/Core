namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Security.Cryptography;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class HMACBundlerTest
    {
        private HMACBundler _target;

        private Mock<IHMACFactory> _hmacFactory = new Mock<IHMACFactory>();
        private Mock<IHMAC> _hmac;

        private RNGCryptoServiceProvider _cryptoRandom;

        private byte[] _key;
        private byte[] _iv;
        private byte[] _cipherText;
        private byte[] _nonSecretPayload;
        private byte[] _encryptedMessage;

        private const int ivLength = 16;
        private const int nonSecretPayloadLength = 5;

        [TestInitialize]
        public void Initialize()
        {
            _hmacFactory = new Mock<IHMACFactory>();
            _hmac = new Mock<IHMAC>();

            _cryptoRandom = new RNGCryptoServiceProvider();

            _key = CreateBytes(64);
            _iv = CreateBytes(ivLength);
            _cipherText = CreateBytes(10);
            _nonSecretPayload = CreateBytes(nonSecretPayloadLength);

            _hmacFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(_hmac.Object);
            _hmacFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(_hmac.Object);

            _target = new HMACBundler(_hmacFactory.Object);

            _encryptedMessage = CreateBytes(ivLength + nonSecretPayloadLength + _target.TagLength + 10);
        }

        [TestMethod]
        public void Bundle_NullKey_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Bundle(null, _iv, _cipherText, _nonSecretPayload));
        }

        [TestMethod]
        public void Bundle_ValidBundle()
        {
            IHMACBundle result = _target.Bundle(_key, _iv, _cipherText, _nonSecretPayload);

            Assert.IsNotNull(result.EncryptedMessage);
            Assert.AreEqual(_iv, result.IV);
            Assert.AreEqual(_nonSecretPayload, result.NonSecretPayload);
            Assert.AreEqual(_cipherText, result.CipherText);
            Assert.IsNotNull(result.SentTag);
        }

        [TestMethod]
        public void Bundle_AllowNullNonSecretPayload()
        {
            _target.Bundle(_key, _iv, _cipherText, null);
        }

        [TestMethod]
        public void Bundle_AllowNullIV()
        {
            _target.Bundle(_key, _iv, _cipherText, _nonSecretPayload);
        }

        [TestMethod]
        public void Bundle_NullCipherText_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Bundle(_key, _iv, null, _nonSecretPayload));
        }

        [TestMethod]
        public void UnBundle_AllowNonSecretPayloadLengthOfZero()
        {
            byte[] encryptedMessageNoNonSecretPayload = CreateBytes(ivLength + _target.TagLength + 10);

            _target.UnBundle(encryptedMessageNoNonSecretPayload, 0, ivLength);
        }

        [TestMethod]
        public void UnBundle_NullEncryptedMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.UnBundle(null, nonSecretPayloadLength, ivLength));
        }

        [TestMethod]
        public void UnBundle_AllowIVLengthOfZero()
        {
            byte[] encryptedMessageNoIV = CreateBytes(nonSecretPayloadLength + _target.TagLength + 10);

            _target.UnBundle(encryptedMessageNoIV, nonSecretPayloadLength, 0);
        }

        [TestMethod]
        public void UnBundle_NonSecretPayloadLengthLessThanZero_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.UnBundle(_encryptedMessage, -1, ivLength));
        }

        [TestMethod]
        public void UnBundle_IVLengthLessThanZero_ThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => _target.UnBundle(_encryptedMessage, nonSecretPayloadLength, -1));
        }

        [TestMethod]
        public void UnBundle_EncryptedMessageTooSmall_ThrowException()
        {
            byte[] encryptedMessageTooSmall = CreateBytes((ivLength + nonSecretPayloadLength + _target.TagLength) - 1);

            Assert.ThrowsException<ArgumentException>(() => _target.UnBundle(encryptedMessageTooSmall, nonSecretPayloadLength, ivLength));
        }

        [TestMethod]
        public void UnBundle_AllowNonSecretPayloadAndIVLengthOfZero()
        {
            byte[] encryptedMessageNoNonSecretPayloadNorIV = CreateBytes(_target.TagLength + 10);

            _target.UnBundle(encryptedMessageNoNonSecretPayloadNorIV, 0, 0);
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
