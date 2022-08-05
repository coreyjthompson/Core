namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Security.Cryptography;

    using MEI.Security.Cryptography;

    using NodaTime;

    public static class MEISecurityCryptographyServiceCollectionExtensions
    {
        public static IServiceCollection AddMEISecurityCryptography(this IServiceCollection services)
        {
            return services
                .AddScoped<IKeyVersionEncryptorAndBundler, KeyVersionEncryptorAndBundler>()
                .AddScoped<ISymmetricEncryptor>(provider => new SymmetricEncryptor(new AesManaged()))
                .AddScoped<IHMACBundler, HMACBundler>()
                .AddScoped<IKeyVersionHMACBundler, KeyVersionHMACBundler>()
                .AddScoped<IPasswordEncryptorAndBundler, PasswordEncryptorAndBundler>()
                .AddScoped<IHMACFactory, HMACFactory>()
                .AddSingleton<IClock>(SystemClock.Instance);
        }
    }

    [Obsolete("Use 'MEISecurityCryptographyServiceCollectionExtensions' instead (Note the spelling difference). This one will be removed in the next version.")]
    public static class MEISecurityCryptogprahyServiceCollectionExtensions
    {
        public static IServiceCollection AddMEISecurityCryptography(this IServiceCollection services)
        {
            return services
                .AddScoped<IKeyVersionEncryptorAndBundler, KeyVersionEncryptorAndBundler>()
                .AddScoped<ISymmetricEncryptor>(provider => new SymmetricEncryptor(new AesManaged()))
                .AddScoped<IHMACBundler, HMACBundler>()
                .AddScoped<IKeyVersionHMACBundler, KeyVersionHMACBundler>()
                .AddScoped<IPasswordEncryptorAndBundler, PasswordEncryptorAndBundler>()
                .AddScoped<IHMACFactory, HMACFactory>()
                .AddSingleton<IClock>(SystemClock.Instance);
        }
    }
}
