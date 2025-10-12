using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.ModuleCore;
using HomeBook.Frontend.Properties;
using HomeBook.Frontend.Provider;
using HomeBook.Frontend.Services;
using HomeBook.Frontend.Setup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.FeatureManagement;

namespace HomeBook.Frontend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendUiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(configuration)
            .AddLocalization()
            .AddFeatureManagement();

        services.AddSingleton<ISetupService, SetupService>();
        services.AddSingleton<IStartupService, StartupService>();

        services.AddSingleton<ILocalizationProvider, LocalizationProvider>(x =>
        {
            Type localizerType = typeof(IStringLocalizer<>).MakeGenericType(typeof(LocalizationStrings));
            IStringLocalizer localizer = (IStringLocalizer)x.GetRequiredService(localizerType);
            return new LocalizationProvider(localizer);
        });

        services.AddSingleton<IWidgetFactory, WidgetFactory>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        return services;
    }
}
