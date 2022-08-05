namespace MEI.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Microsoft.Extensions.Options;

    public class VaultKeyManager
        : IKeyManager
    {
        private readonly IKeyVaultFactory _keyVaultFactory;
        private readonly IOptions<VaultKeyManagerOptions> _options;
        private bool _hasExtractedKeys;
        private IReadOnlyDictionary<int, byte[]> _authKeys;
        private IReadOnlyDictionary<int, byte[]> _cryptKeys;
        private int _currentAuthKeyVersionNumber;
        private int _currentCryptKeyVersionNumber;

        public VaultKeyManager(IKeyVaultFactory keyVaultFactory, IOptions<VaultKeyManagerOptions> options)
        {
            _keyVaultFactory = keyVaultFactory;
            _options = options;

            _authKeys = new Dictionary<int, byte[]>();
            _cryptKeys = new Dictionary<int, byte[]>();
        }

        public int CurrentAuthKeyVersionNumber
        {
            get
            {
                if (!_hasExtractedKeys)
                {
                    ExtractKeys();

                    _hasExtractedKeys = true;
                }

                return _currentAuthKeyVersionNumber;
            }
        }

        public int CurrentCryptKeyVersionNumber
        {
            get
            {
                if (!_hasExtractedKeys)
                {
                    ExtractKeys();

                    _hasExtractedKeys = true;
                }

                return _currentCryptKeyVersionNumber;
            }
        }

        public IReadOnlyDictionary<int, byte[]> AuthKeys
        {
            get
            {
                if (!_hasExtractedKeys)
                {
                    ExtractKeys();

                    _hasExtractedKeys = true;
                }

                return _authKeys;
            }
        }

        public IReadOnlyDictionary<int, byte[]> CryptKeys
        {
            get
            {
                if (!_hasExtractedKeys)
                {
                    ExtractKeys();

                    _hasExtractedKeys = true;
                }

                return _cryptKeys;
            }
        }

        public void ExtractKeys()
        {
            using (IKeyVault keyVault = _keyVaultFactory.Create(_options.Value.AuthClientId, _options.Value.AuthSecret))
            {
                // SECURITY: DO NOT USE IN PRODUCTION CODE; FOR TEST PURPOSES ONLY
                /*ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;*/

                var authKeys = new Dictionary<int, byte[]>();

                foreach (KeyValuePair<string, string> keyId in _options.Value.AuthKeyIds)
                {
                    string authKeyId = string.Format("{0}secrets/{1}/{2}", _options.Value.VaultUrl, _options.Value.AuthKeyName, keyId.Value);

                    authKeys.Add(Convert.ToInt32(keyId.Key), Convert.FromBase64String(keyVault.GetSecretById(authKeyId)));
                }

                _authKeys = new ReadOnlyDictionary<int, byte[]>(authKeys);

                var cryptKeys = new Dictionary<int, byte[]>();

                foreach (KeyValuePair<string, string> keyId in _options.Value.CryptKeyIds)
                {
                    string cryptKeyId = string.Format("{0}secrets/{1}/{2}", _options.Value.VaultUrl, _options.Value.CryptKeyName, keyId.Value);

                    cryptKeys.Add(Convert.ToInt32(keyId.Key), Convert.FromBase64String(keyVault.GetSecretById(cryptKeyId)));
                }

                _cryptKeys = new ReadOnlyDictionary<int, byte[]>(cryptKeys);
            }

            _currentAuthKeyVersionNumber = !string.IsNullOrEmpty(_options.Value.CurrentAuthKeyVersion)
                ? Convert.ToInt32(_options.Value.CurrentAuthKeyVersion)
                : _authKeys.Select(authKey => authKey.Key).Concat(new[] { 0 }).Max();

            _currentCryptKeyVersionNumber = !string.IsNullOrEmpty(_options.Value.CurrentCryptKeyVersion)
                ? Convert.ToInt32(_options.Value.CurrentCryptKeyVersion)
                : _cryptKeys.Select(cryptKey => cryptKey.Key).Concat(new[] { 0 }).Max();
        }
    }
}
