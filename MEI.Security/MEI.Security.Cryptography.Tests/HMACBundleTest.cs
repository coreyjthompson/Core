namespace MEI.Security.Cryptography.Tests
{
    using System.Security.Cryptography;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class HMACBundleTest
    {
        private HMACBundle _target;

        private Mock<IHMACFactory> _hmacFactory;
        private Mock<IHMAC> _hmac;

        private RNGCryptoServiceProvider _cryptoRandom;

        private byte[] _key;
        private byte[] _sentTag;

        [TestInitialize]
        public void Initialize()
        {
            _hmacFactory = new Mock<IHMACFactory>();
            _hmac = new Mock<IHMAC>();

            _cryptoRandom = new RNGCryptoServiceProvider();

            _hmacFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(_hmac.Object);
            _hmacFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(_hmac.Object);

            byte[] encryptedMessage = CreateBytes(50);
            byte[] iv = CreateBytes(8);
            byte[] nonSecretPayload = CreateBytes(16);
            byte[] cipherText = CreateBytes(10);
            _sentTag = CreateBytes(8);
            _key = CreateBytes(8);

            _target = new HMACBundle(_hmacFactory.Object, encryptedMessage, iv, nonSecretPayload, cipherText, _sentTag);
        }

        [TestMethod]
        public void IsValid_TooShortEncryptedMessage_ReturnFalse()
        {
            byte[] encryptedMessage = CreateBytes(1);
            byte[] iv = CreateBytes(8);
            byte[] nonSecretPayload = CreateBytes(16);
            byte[] cipherText = CreateBytes(10);
            byte[] sentTag = CreateBytes(8);
            var target = new HMACBundle(_hmacFactory.Object, encryptedMessage, iv, nonSecretPayload, cipherText, sentTag);

            bool result = target.IsValid(_key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_TagsDoNotMatch_ReturnFalse()
        {
            var computedHash = new byte[_sentTag.Length];
            _sentTag.CopyTo(computedHash, 0);
            computedHash[1] += 1;
            _hmac.Setup(x => x.ComputeHash(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(computedHash);

            bool result = _target.IsValid(_key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_TagsDoMatch_ReturnTrue()
        {
            var computedHash = new byte[_sentTag.Length];
            _sentTag.CopyTo(computedHash, 0);
            _hmac.Setup(x => x.ComputeHash(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(computedHash);

            bool result = _target.IsValid(_key);

            Assert.IsTrue(result);
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(new byte[size]);

            return buff;
        }
    }
}
