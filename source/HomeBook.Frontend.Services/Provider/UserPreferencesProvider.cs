using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;

namespace HomeBook.Frontend.Services.Provider;

/// <inheritdoc />
public class UserPreferencesProvider(
    IAuthenticationService authenticationService,
    BackendClient backendClient) : IUserPreferencesProvider
{
    /// <inheritdoc />
    public async Task<string?> GetLocaleAsync(CancellationToken cancellationToken = default)
    {
        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        GetUserPreferenceLocaleResponse? response = await backendClient.User.Preferences.Locale.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);

        return response?.Locale;
    }

    /// <inheritdoc />
    public async Task SetLocaleAsync(string locale,
        CancellationToken cancellationToken = default)
    {
        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        await backendClient.User.Preferences.Locale.PostAsync(new UpdateUserPreferenceLocaleRequest()
            {
                Locale = locale
            },
            x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);
    }
}
