using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class LicenseAgreementSetupStep : ComponentBase, ISetupStep
{
    private bool _isLoading = false;
    private string? _errorMessage = null;
    private Dictionary<string, string> _licenses = new();

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
                // DE => Der Server hat keine gültige Version zurückgegeben.
                throw new SetupCheckException("Server did not return valid licenses.");

            _licenses.Clear();
            // foreach (var license in licensesResponse.Licenses)
            // {
            //     _licenses[license.Name] = license.Text;
            // }
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            throw new SetupCheckException("Unknown Server Error while loading Licenses.");
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
    }

    private async Task StepErrorAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(false, true, cancellationToken);
    }

    private async Task StepSuccessAsync(CancellationToken cancellationToken = default)
    {
        await SetupService.SetStepStatusAsync(false, false, cancellationToken);
        await Task.Delay(5000, cancellationToken);
        await SetupService.SetStepStatusAsync(true, false, cancellationToken);
    }
}
