namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Security.Cryptography;

    using FizzWare.NBuilder;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HMACFactoryTest
    {
        private HMACFactory _target;

        private RNGCryptoServiceProvider _cryptoRandom;

        private byte[] _key;
        private string _kind;

        [TestInitialize]
        public void Initialize()
        {
            _cryptoRandom = new RNGCryptoServiceProvider();

            _kind = "SHA256";
            _key = CreateBytes(16);

            _target = new HMACFactory();
        }

        [TestMethod]
        public void Create_InvalidType_ThrowException()
        {
            string badKind = new RandomGenerator().NextString(5, 5);

            Assert.ThrowsException<InvalidOperationException>(() => _target.Create(badKind));
        }

        [TestMethod]
        public void Create_ReturnValidHMAC()
        {
            IHMAC result = _target.Create(_kind);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create2_InvalidType_ThrowException()
        {
            string badKind = new RandomGenerator().NextString(5, 5);

            Assert.ThrowsException<InvalidOperationException>(() => _target.Create(badKind, _key));
        }

        [TestMethod]
        public void Create2_ReturnValidHMAC()
        {
            IHMAC result = _target.Create(_kind, _key);

            Assert.IsNotNull(result);
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
