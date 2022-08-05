namespace MEI.Security.Cryptography
{
    using System.Configuration;

    using Microsoft.Extensions.Configuration;

    public class LegacyConfigurationProvider
        : ConfigurationProvider, IConfigurationSource
    {
        private readonly string _authKeyName;
        private readonly string _cryptKeyName;

        public LegacyConfigurationProvider(string authKeyName, string cryptKeyName)
        {
            _authKeyName = authKeyName;
            _cryptKeyName = cryptKeyName;
        }

        public override void Load()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                Data.Add($"ConnectionStrings:{connectionString.Name}", connectionString.ConnectionString);
            }

            foreach (string settingKey in ConfigurationManager.AppSettings.AllKeys)
            {
                if (settingKey.StartsWith(_authKeyName))
                {
                    string keyIndex = settingKey.Replace(_authKeyName, string.Empty);
                    Data.Add("AuthKeyIds:" + keyIndex, ConfigurationManager.AppSettings[settingKey]);
                }
                else if (settingKey.StartsWith(_cryptKeyName))
                {
                    string keyIndex = settingKey.Replace(_cryptKeyName, string.Empty);
                    Data.Add("CryptKeyIds:" + keyIndex, ConfigurationManager.AppSettings[settingKey]);
                }
                else
                {
                    Data.Add(settingKey, ConfigurationManager.AppSettings[settingKey]);
                }
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this;
        }
    }
}
