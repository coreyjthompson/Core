namespace MEI.Security.Cryptography
{
    using System;
    using System.Security.Cryptography;

    public interface IHMACFactory
    {
        IHMAC Create(string kind);

        IHMAC Create(string kind, byte[] key);
    }

    public class HMACFactory
        : IHMACFactory
    {
        public IHMAC Create(string kind)
        {
            switch (kind)
            {
                case "SHA256":
                    return new HMACWrapper(new HMACSHA256());
            }

            throw new InvalidOperationException(string.Format(Resource1.hmac_kind_is_not_valid___0_, kind));
        }

        public IHMAC Create(string kind, byte[] key)
        {
            switch (kind)
            {
                case "SHA256":
                    return new HMACWrapper(new HMACSHA256(key));
            }

            throw new InvalidOperationException(string.Format(Resource1.hmac_kind_is_not_valid___0_, kind));
        }
    }
}
