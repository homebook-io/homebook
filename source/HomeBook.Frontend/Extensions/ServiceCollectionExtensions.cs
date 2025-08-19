using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Properties;
using HomeBook.Frontend.Provider;
using HomeBook.Frontend.Setup;
using Microsoft.Extensions.Localization;

namespace HomeBook.Frontend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendUiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ISetupService, SetupService>();

        services.AddLocalization();
        services.AddSingleton<ILocalizationProvider, LocalizationProvider>(x =>
        {
            Type localizerType = typeof(IStringLocalizer<>).MakeGenericType(typeof(LocalizationStrings));
            IStringLocalizer localizer = (IStringLocalizer)x.GetRequiredService(localizerType);
            return new LocalizationProvider(localizer);
        });

        return services;
    }
}
