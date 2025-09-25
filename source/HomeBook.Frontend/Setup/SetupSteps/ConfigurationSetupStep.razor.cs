using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Core.Models.Setup;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class ConfigurationSetupStep : ComponentBase, ISetupStep
{
    private bool _isChecking = false;
    private bool _configurationIsOk = false;
    private bool _preConfigured = false;
    private string? _errorMessage = null;
    private HomebookConfigurationViewModel _homebookConfiguration = new();

    private readonly List<LanguageViewModel> _availableLanguages =
    [
        new("EN", "Englisch (English)"),
        new("DE", "German (Deutsch)")
    ];

    public string Key { get; } = nameof(ConfigurationSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        try
        {
            // set default
            _homebookConfiguration.InstanceName = "My HomeBook";

            _isChecking = true;
            StateHasChanged();

            CancellationToken cancellationToken = CancellationToken.None;

            await BackendClient.Setup.Configuration.GetAsync(x =>
                {
                },
                cancellationToken);

            // if request is successful, user is already set up
            _configurationIsOk = true;
            _preConfigured = true;
        }
        catch (ApiException err) when (err.ResponseStatusCode == 404)
        {
            // user not found, needs to be set up
            _configurationIsOk = false;
            _preConfigured = false;
        }
        catch (Exception err)
        {
            _errorMessage = err.Message;
        }
        finally
        {
            _isChecking = false;
            StateHasChanged();
        }
    }

    private async Task OnValidSubmit()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        await SetupService.SetStorageValueAsync("HOMEBOOK_CONFIGURATION_NAME",
            _homebookConfiguration.InstanceName,
            cancellationToken);
        await SetupService.SetStorageValueAsync("HOMEBOOK_CONFIGURATION_DEFAULT_LANG",
            _homebookConfiguration.InstanceDefaultLang,
            cancellationToken);

        _errorMessage = null;
        _configurationIsOk = true;
        _preConfigured = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnCountdownFinishedAsync()
    {
        if (!_configurationIsOk)
            return;

        CancellationToken cancellationToken = CancellationToken.None;

        await StepSuccessAsync(cancellationToken);
        await InvokeAsync(StateHasChanged);
    }

    private async Task StepErrorAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(false, true, cancellationToken);
    }

    private async Task StepSuccessAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(true, false, cancellationToken);
    }
}
