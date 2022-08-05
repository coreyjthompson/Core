namespace MEI.Security.Cryptography.Tests
{
    using System.Security.Cryptography;

    using FizzWare.NBuilder;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PasswordKeyGeneratorTest
    {
        private PasswordKeyGenerator _target;

        private RNGCryptoServiceProvider _cryptoRandom;

        private string _password;
        private int _saltSize;
        private int _iterations;
        private int _keySize;

        [TestInitialize]
        public void Initialize()
        {
            _cryptoRandom = new RNGCryptoServiceProvider();

            _password = new RandomGenerator().NextString(25, 25);
            _saltSize = 8;
            _iterations = 100;
            _keySize = 16;

            _target = new PasswordKeyGenerator();
        }

        [TestMethod]
        public void GenerateKeys_ReturnValidKeys()
        {
            PasswordKeys result = _target.GenerateKeys(_password, _saltSize, _iterations, _keySize);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AuthKey);
            Assert.AreEqual(result.AuthKey.Length, _keySize);
            Assert.IsNotNull(result.AuthKeySalt);
            Assert.AreEqual(result.AuthKeySalt.Length, _saltSize);
            Assert.IsNotNull(result.CryptKey);
            Assert.AreEqual(result.CryptKey.Length, _keySize);
            Assert.IsNotNull(result.CryptKeySalt);
            Assert.AreEqual(result.CryptKeySalt.Length, _saltSize);
        }

        [TestMethod]
        public void GenerateKeys2_ReturnValidKeys()
        {
            byte[] cryptKeySalt = CreateBytes(8);
            byte[] authKeySalt = CreateBytes(8);

            PasswordKeys result = _target.GenerateKeys(_password, cryptKeySalt, authKeySalt, _iterations, _keySize);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AuthKey);
            Assert.AreEqual(result.AuthKey.Length, _keySize);
            Assert.AreEqual(authKeySalt, result.AuthKeySalt);
            Assert.IsNotNull(result.CryptKey);
            Assert.AreEqual(result.CryptKey.Length, _keySize);
            Assert.AreEqual(cryptKeySalt, result.CryptKeySalt);
        }

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _cryptoRandom.GetBytes(buff);

            return buff;
        }
    }
}
