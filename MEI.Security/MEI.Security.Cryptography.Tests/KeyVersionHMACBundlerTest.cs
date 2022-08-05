namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using NodaTime;

    [TestClass]
    public class KeyVersionHMACBundlerTest
    {
        private KeyVersionHMACBundler _target;

        private Mock<IHMACFactory> _hmacFactory;
        private Mock<IHMAC> _hmac;

        private RNGCryptoServiceProvider _cryptoRandom;

        private const int authKeyVersionNumber = 1;
        private const int cryptKeyVersionNumber = 1;

        private byte[] _key;
        private byte[] _iv;
        private byte[] _cipherText;
        private Instant _encryptionInstant;

        [TestInitialize]
        public void Initialize()
        {
            _cryptoRandom = new RNGCryptoServiceProvider();

            _hmacFactory = new Mock<IHMACFactory>();
            _hmac = new Mock<IHMAC>();

            _key = CreateBytes(64);
            _iv = CreateBytes(16);
            _cipherText = CreateBytes(10);
            _encryptionInstant = SystemClock.Instance.GetCurrentInstant();

            _hmacFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(_hmac.Object);
            _hmacFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(_hmac.Object);

            _target = new KeyVersionHMACBundler(_hmacFactory.Object);
        }

        [TestMethod]
        public void Bundle_ValidBundle()
        {
            IKeyVersionHMACBundle result = _target.Bundle(_key, _iv, _cipherText, authKeyVersionNumber, cryptKeyVersionNumber, _encryptionInstant);

            Assert.AreEqual(authKeyVersionNumber, result.AuthKeyVersionNumber);
            Assert.AreEqual(cryptKeyVersionNumber, result.CryptKeyVersionNumber);
            Assert.AreEqual(_encryptionInstant, result.EncryptionInstant);
        }

        [TestMethod]
        public void UnBundle_ValidBundle()
        {
            byte[] encryptedMessage = CreateEncryptedBundle();

            IKeyVersionHMACBundle result = _target.UnBundle(encryptedMessage, 16, 8);

            Assert.AreEqual(authKeyVersionNumber, result.AuthKeyVersionNumber);
            Assert.AreEqual(cryptKeyVersionNumber, result.CryptKeyVersionNumber);
            Assert.AreEqual(_encryptionInstant, result.EncryptionInstant);
        }

        private byte[] CreateEncryptedBundle()
        {
            byte[] ivCipherAndSentTagBytes = CreateBytes(50);
            byte[] authKeyVersionNumberBytes = BitConverter.GetBytes(authKeyVersionNumber);
            byte[] cryptKeyVersionNumberBytes = BitConverter.GetBytes(cryptKeyVersionNumber);
            byte[] encryptionInstantBytes = BitConverter.GetBytes(_encryptionInstant.ToUnixTimeTicks());

            return authKeyVersionNumberBytes.Concat(cryptKeyVersionNumberBytes)
                .Concat(encryptionInstantBytes).Concat(ivCipherAndSentTagBytes).ToArray();
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
