namespace MEI.Security.Cryptography
{
    public class PasswordKeys
    {
        public byte[] CryptKey { get; set; }

        public byte[] AuthKey { get; set; }

        public byte[] CryptKeySalt { get; set; }

        public byte[] AuthKeySalt { get; set; }
    }
}
