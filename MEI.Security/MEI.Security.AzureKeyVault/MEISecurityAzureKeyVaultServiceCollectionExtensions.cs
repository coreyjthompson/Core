namespace Microsoft.Extensions.DependencyInjection
{
    using MEI.Security.AzureKeyVault;
    using MEI.Security.Cryptography;

    public static class MEISecurityAzureKeyVaultServiceCollectionExtensions
    {
        public static IServiceCollection AddMEISecurityAzureKeyVault(this IServiceCollection services)
        {
            return services
                .AddScoped<IKeyVaultFactory, AzureKeyVaultFactory>()
                .AddSingleton<IKeyManager, VaultKeyManager>();
        }
    }
}
