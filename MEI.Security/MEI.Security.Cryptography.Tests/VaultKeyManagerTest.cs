namespace MEI.Security.Cryptography.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class VaultKeyManagerTest
    {
        private VaultKeyManager _target;

        private Mock<IKeyVaultFactory> _keyVaultFactory;
        private Mock<IKeyVault> _keyVault;

        private VaultKeyManagerOptions _options;

        private readonly RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        [TestInitialize]
        public void Initialize()
        {
            _keyVaultFactory = new Mock<IKeyVaultFactory>();
            var mockOptions = new Mock<IOptions<VaultKeyManagerOptions>>();
            _keyVault = new Mock<IKeyVault>();

            _options = new VaultKeyManagerOptions();

            mockOptions.SetupGet(x => x.Value).Returns(_options);

            _keyVaultFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(_keyVault.Object);

            _target = new VaultKeyManager(_keyVaultFactory.Object, mockOptions.Object);
        }

        /*[TestMethod]
        public void CurrentAuthKeyVersionNumber_KeysNotExtractedYet_ExtractThem()
        {
            _keyVault.SetupSequence(x => x.GetSecretById(It.IsAny<string>())).Returns(Convert.ToBase64String(CreateBytes(10)));
            _options.AuthKeyIds = new Dictionary<string, string>
                                  {
                                      {"1", "2" }
                                  };
            _options.CryptKeyIds = new Dictionary<string, string>
                                   {
                                       { "3", "4" }
                                   };

            int currentAuthKeyVersionNumber = _target.CurrentAuthKeyVersionNumber;

            Assert.IsTrue(_target.AuthKeys.Count > 0);
            Assert.IsTrue(_target.CryptKeys.Count > 0);
        }*/

        private byte[] CreateBytes(int size)
        {
            var buff = new byte[size];
            _random.GetBytes(buff);

            return buff;
        }
    }
}
