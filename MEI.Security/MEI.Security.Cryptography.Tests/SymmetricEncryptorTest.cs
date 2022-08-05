namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Security.Cryptography;

    using FizzWare.NBuilder;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class SymmetricEncryptorTest
    {
        private SymmetricEncryptor _target;

        private Mock<SymmetricAlgorithm> _algorithm;
        private Mock<ICryptoTransform> _cryptoTransform;

        private RNGCryptoServiceProvider _cryptoRandom;

        private byte[] _key;
        private byte[] _iv;
        private string _secretMessage;
        private int _keySize;

        [TestInitialize]
        public void Initialize()
        {
            _cryptoTransform = new Mock<ICryptoTransform>();
            _algorithm = new Mock<SymmetricAlgorithm>();

            _cryptoRandom = new RNGCryptoServiceProvider();

            _keySize = 256;
            _key = CreateBytes(_keySize / 8);
            _iv = CreateBytes(_keySize / 8);

            string plainSecretMessage = new RandomGenerator().NextString(10, 10);
            _secretMessage = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainSecretMessage));

            _cryptoTransform.SetupGet(x => x.InputBlockSize).Returns(16);
            _cryptoTransform.SetupGet(x => x.OutputBlockSize).Returns(16);
            _cryptoTransform.SetupGet(x => x.CanReuseTransform).Returns(true);
            _cryptoTransform.SetupGet(x => x.CanTransformMultipleBlocks).Returns(true);
            _cryptoTransform.Setup(x =>
                x.TransformBlock(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<int>())).Returns(10);
            _cryptoTransform.Setup(x => x.TransformFinalBlock(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(CreateBytes(10));

            _algorithm.SetupGet(x => x.KeySize).Returns(_keySize);
            _algorithm.SetupGet(x => x.IV).Returns(_iv);
            _algorithm.Setup(x => x.CreateEncryptor(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(_cryptoTransform.Object);
            _algorithm.Setup(x => x.CreateDecryptor(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(_cryptoTransform.Object);

            _target = new SymmetricEncryptor(_algorithm.Object);
        }

        [TestMethod]
        public void Encrypt_NullSecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(_key, _iv, (string)null));
        }

        [TestMethod]
        public void Encrypt_EmptyStringSecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(_key, _iv, string.Empty));
        }

        [TestMethod]
        public void Encrypt_NullKey_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Encrypt(null, _iv, _secretMessage));
        }

        [TestMethod]
        public void Encrypt_IncorrectKeySize_ThrowException()
        {
            byte[] key = CreateBytes(_keySize + 1);

            Assert.ThrowsException<ArgumentException>(() => _target.Encrypt(key, _iv, _secretMessage));
        }

        [TestMethod]
        public void Encrypt_ValidInput_ReturnValidEncryption()
        {
            string result = _target.Encrypt(_key, _iv, _secretMessage);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Decrypt_NullSecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(_key, _iv, (string)null));
        }

        [TestMethod]
        public void Decrypt_EmptySecretMessage_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(_key, _iv, string.Empty));
        }

        [TestMethod]
        public void Decrypt_NullKey_ThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _target.Decrypt(null, _iv, _secretMessage));
        }

        [TestMethod]
        public void Decrypt_SecretMessageNotBase64Format_ThrowException()
        {
            string secretMessage = new RandomGenerator().NextString(10, 10) + "!";

            Assert.ThrowsException<FormatException>(() => _target.Decrypt(_key, _iv, secretMessage));
        }

        [TestMethod]
        public void Decrypt_ValidInput_ReturnValidDecryption()
        {
            string result = _target.Decrypt(_key, _iv, _secretMessage);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Decrypt_KeyLengthDoesNotEqualKeySize_ThrowException()
        {
            var key = new byte[_algorithm.Object.KeySize - 1];

            Assert.ThrowsException<ArgumentException>(() => _target.Decrypt(key, _iv, Convert.FromBase64String(_secretMessage)));
        }

        [TestMethod]
        public void BlockSize_ShouldBeSameLengthAsAlgorithm()
        {
            Assert.AreEqual(_algorithm.Object.BlockSize, _target.BlockSize);
        }

        [TestMethod]
        public void GenerateIV_IVIsValid()
        {
            _target.GenerateIV();

            Assert.IsNotNull(_target.IV);
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
