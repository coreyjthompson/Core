namespace MEI.Security.Cryptography
{
    using System.Collections.Generic;

    public class VaultKeyManagerOptions
    {
        public VaultKeyManagerOptions()
        {
            AuthKeyIds = new Dictionary<string, string>();
            CryptKeyIds = new Dictionary<string, string>();
        }

        public string AuthClientId { get; set; }

        public string AuthSecret { get; set; }

        public string VaultUrl { get; set; }

        public string CurrentAuthKeyVersion { get; set; }

        public Dictionary<string, string> AuthKeyIds { get; set; }

        public string AuthKeyName { get; set; }

        public string CurrentCryptKeyVersion { get; set; }

        public Dictionary<string, string> CryptKeyIds { get; set; }

        public string CryptKeyName { get; set; }
    }
}
