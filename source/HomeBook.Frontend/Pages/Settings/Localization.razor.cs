using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Core.Models.Setup;
using HomeBook.Frontend.Core.Models.UserPreferences;
using HomeBook.Frontend.Mappings;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings;

public partial class Localization : ComponentBase
{
    private MudForm _form = new();
    private bool _isValid;
    private bool _isLoading;
    private readonly UserPreferenceLocalizationViewModel _configurationModel = new();
    private readonly List<LanguageViewModel> _availableLanguages = [];

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        StateHasChanged();

        CancellationToken cancellationToken = CancellationToken.None;

        // load needed data
        await LoadAvailableLocalesAsync(cancellationToken);

        // load current configuration
        await LoadCurrentLocaleAsync(cancellationToken);

        _isLoading = false;
        StateHasChanged();
    }

    private async Task LoadAvailableLocalesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // TODO: move loading locales to service
            GetLocalesResponse? localeResponse = await BackendClient.Platform.Locales.GetAsync(x =>
                {
                },
                cancellationToken);

            if (localeResponse is not null)
            {
                List<LocaleResponse>? locales = localeResponse.Locales;
                _availableLanguages.Clear();
                foreach (LocaleResponse locale in (locales ?? []).OfType<LocaleResponse>())
                {
                    _availableLanguages.Add(locale.ToViewModel());
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading instance configuration: {ex.Message}", Severity.Error);
        }
    }

    private async Task LoadCurrentLocaleAsync(CancellationToken cancellationToken)
    {
        try
        {
            string? locale = await UserPreferencesProvider.GetLocaleAsync(cancellationToken);

            _configurationModel.Locale = locale;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading instance configuration: {ex.Message}", Severity.Error);
        }
    }

    private async Task UpdateInstanceConfigurationAsync()
    {
        if (!_isValid)
            return;

        try
        {
            _isLoading = true;

            // Validate form before proceeding
            await _form.Validate();
            if (!_form.IsValid)
                return;

            await UpdateLocaleAsync(_configurationModel.Locale);

            Snackbar.Add("User preferences updated", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user preferences: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task UpdateLocaleAsync(string locale)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        await UserPreferencesProvider.SetLocaleAsync(locale,
            cancellationToken);

        await LocalizationService.SetCultureAsync(locale,
            true,
            cancellationToken);
    }
}
