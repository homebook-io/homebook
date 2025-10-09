using System.Globalization;
using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HomeBook.Frontend.Services.Services;

/// <inheritdoc />
public class LocalizationService(
    IAuthenticationService authenticationService,
    IInstanceManagementProvider instanceManagementProvider,
    BackendClient backendClient,
    NavigationManager navigationManager,
    IJSRuntime jsRuntime) : ILocalizationService, IAsyncDisposable
{
    private string _defaultLocale;

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string defaultLocale = await instanceManagementProvider.GetDefaultLocaleAsync(cancellationToken);
        _defaultLocale = defaultLocale;

        await SetCultureAsync(_defaultLocale,
            false,
            cancellationToken);

        authenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        authenticationService.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    /// <inheritdoc />
    public async Task<CultureInfo?> GetCultureAsync(CancellationToken cancellationToken = default)
    {
        string culture = await jsRuntime.InvokeAsync<string>("localStorage.getItem",
            cancellationToken,
            JsLocalStorageKeys.UserPreferenceLocale);

        if (!string.IsNullOrEmpty(culture))
            return new CultureInfo(culture);

        return null;
    }

    /// <inheritdoc />
    public async Task SetCultureAsync(string culture,
        bool forceLoad = true,
        CancellationToken cancellationToken = default) =>
        await SetCultureAsync(new CultureInfo(culture), forceLoad, cancellationToken);

    /// <inheritdoc />
    public async Task SetCultureAsync(CultureInfo cultureInfo,
        bool forceLoad = true,
        CancellationToken cancellationToken = default)
    {
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        await jsRuntime.InvokeVoidAsync("localStorage.setItem",
            cancellationToken,
            JsLocalStorageKeys.UserPreferenceLocale,
            cultureInfo.IetfLanguageTag);

        if (forceLoad)
            navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
    }

    private void OnAuthenticationStateChanged(bool isAuthenticated)
    {
        if (!isAuthenticated)
            Task.Run(() => SetCultureAsync(_defaultLocale, true));
        else
            Task.Run(() => LoadCultureForUserAsync(true));
    }

    private async Task LoadCultureForUserAsync(bool forceReload = true)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        if (!await authenticationService.IsAuthenticatedAsync(cancellationToken))
            return;

        string? locale = null;

        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        GetUserPreferenceLocaleResponse? response = await backendClient.User.Preferences.Locale.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);

        if (response is not null)
            locale = response.Locale;

        if (string.IsNullOrEmpty(locale))
            locale = _defaultLocale;

        await SetCultureAsync(locale!,
            forceReload,
            cancellationToken);
    }
}
