using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Components;
using HomeBook.Frontend.Mappings;
using HomeBook.Frontend.Models.Setup;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;
using MudBlazor;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class LicenseAgreementSetupStep : ComponentBase, ISetupStep
{
    private bool _isLoading = false;
    private string? _errorMessage = null;
    private List<LicenseViewModel> _licenses = [];
    private bool _licensesAccepted = false;

    public string Key { get; } = nameof(LicenseAgreementSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        await LoadLicensesAsync();
    }

    private async Task LoadLicensesAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _isLoading = true;
        _errorMessage = null;
        await InvokeAsync(StateHasChanged);

        try
        {
            GetLicensesResponse? licensesResponse = await BackendClient.Setup.Licenses.GetAsync(x =>
                {
                },
                cancellationToken
            );

            if (licensesResponse is null)
                throw new SetupCheckException("Server did not return valid licenses.");

            _licensesAccepted = licensesResponse.LicensesAccepted ?? false;
            _licenses.Clear();
            _licenses = (licensesResponse.Licenses ?? [])
                .OrderBy(x => x.Name)
                .Select(license => license.ToViewModel())
                .ToList();
            await InvokeAsync(StateHasChanged);
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            _errorMessage = "Unknown Server Error while loading Licenses: " + err.Message;
            await StepErrorAsync(cancellationToken);
        }
        catch (Exception err)
        {
            _errorMessage = "error while loading licenses: " + err.Message;
            await StepErrorAsync(cancellationToken);
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        if (_licensesAccepted)
        {
            // wait and go to next step
            await SetupService.SetStepStatusAsync(false, false, cancellationToken);
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task StepErrorAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(false, true, cancellationToken);
    }

    private async Task StepSuccessAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(true, false, cancellationToken);
    }

    private async Task OpenLicensesDialogAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        DialogParameters<UiLicenseDialog> parameters = new()
        {
            {
                x => x.Licenses, _licenses
            }
        };

        IDialogReference licenseDialog = await DialogService.ShowAsync<UiLicenseDialog>("HomeBook Licenses",
            parameters,
            new DialogOptions()
            {
                MaxWidth = MaxWidth.Medium, FullWidth = true,
            });

        DialogResult? licenseDialogResult = await licenseDialog.Result;
        if (licenseDialogResult.Canceled)
            return;

        // license is accepted
        await StepSuccessAsync(cancellationToken);
    }

    private async Task OnCountdownFinishedAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        if (_licensesAccepted)
        {
            await StepSuccessAsync(cancellationToken);
        }
    }
}
