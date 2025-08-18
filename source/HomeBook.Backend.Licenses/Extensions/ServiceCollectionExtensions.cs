using HomeBook.Backend.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Licenses.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreLicenses(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ILicenseProvider, LicenseProvider>();

        return services;
    }
}
