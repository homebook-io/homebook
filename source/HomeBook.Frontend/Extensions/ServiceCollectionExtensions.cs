using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.ModuleCore;
using HomeBook.Frontend.Properties;
using HomeBook.Frontend.Provider;
using HomeBook.Frontend.Services;
using HomeBook.Frontend.Setup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace HomeBook.Frontend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendUiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        services.AddSingleton<ISetupService, SetupService>();

        services.AddLocalization();
        services.AddSingleton<ILocalizationProvider, LocalizationProvider>(x =>
        {
            Type localizerType = typeof(IStringLocalizer<>).MakeGenericType(typeof(LocalizationStrings));
            IStringLocalizer localizer = (IStringLocalizer)x.GetRequiredService(localizerType);
            return new LocalizationProvider(localizer);
        });

        services.AddSingleton<IWidgetFactory, WidgetFactory>();

        return services;
    }
}
