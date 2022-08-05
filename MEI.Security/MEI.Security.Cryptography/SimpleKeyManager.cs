namespace MEI.Security.Cryptography
{
    using System;
    using System.Collections.Generic;

    public class SimpleKeyManager
        : IKeyManager
    {
        public SimpleKeyManager(string encryptionKey)
        {
            CryptKeys = new Dictionary<int, byte[]>
                        {
                            { 1, Convert.FromBase64String(encryptionKey) }
                        };

            AuthKeys = CryptKeys;
        }

        public SimpleKeyManager(string encryptionKey, string authenticationKey)
        {
            AuthKeys = new Dictionary<int, byte[]>
                       {
                           { 1, Convert.FromBase64String(encryptionKey) }
                       };

            CryptKeys = new Dictionary<int, byte[]>
                        {
                            { 1, Convert.FromBase64String(authenticationKey) }
                        };
        }

        public int CurrentAuthKeyVersionNumber => 1;

        public int CurrentCryptKeyVersionNumber => 1;
        public IReadOnlyDictionary<int, byte[]> AuthKeys { get; }

        public IReadOnlyDictionary<int, byte[]> CryptKeys { get; }
    }
}
