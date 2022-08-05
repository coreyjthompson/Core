namespace MEI.Security.Cryptography
{
    using System.Security.Cryptography;

    public interface IPasswordKeyGenerator
    {
        PasswordKeys GenerateKeys(string password, int saltSize, int iterations, int keySize);

        PasswordKeys GenerateKeys(string password, byte[] cryptKeySalt, byte[] authKeySalt, int iterations, int keySize);
    }

    public class PasswordKeyGenerator
        : IPasswordKeyGenerator
    {
        public PasswordKeys GenerateKeys(string password, int saltSize, int iterations, int keySize)
        {
            var keys = new PasswordKeys();

            // Use Random Salt to prevent pre-generated weak password attacks.
            using (var generator = new Rfc2898DeriveBytes(password, saltSize, iterations))
            {
                keys.CryptKeySalt = generator.Salt;
                keys.CryptKey = generator.GetBytes(keySize);
            }

            // Deriving separate key, might be less efficient than using HKDF,
            // but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using (var generator = new Rfc2898DeriveBytes(password, saltSize, iterations))
            {
                keys.AuthKeySalt = generator.Salt;
                keys.AuthKey = generator.GetBytes(keySize);
            }

            return keys;
        }

        public PasswordKeys GenerateKeys(string password, byte[] cryptKeySalt, byte[] authKeySalt, int iterations, int keySize)
        {
            var keys = new PasswordKeys
                       {
                           CryptKeySalt = cryptKeySalt,
                           AuthKeySalt = authKeySalt
                       };

            using (var generator = new Rfc2898DeriveBytes(password, cryptKeySalt, iterations))
            {
                keys.CryptKey = generator.GetBytes(keySize);
            }

            using (var generator = new Rfc2898DeriveBytes(password, authKeySalt, iterations))
            {
                keys.AuthKey = generator.GetBytes(keySize);
            }

            return keys;
        }
    }
}
