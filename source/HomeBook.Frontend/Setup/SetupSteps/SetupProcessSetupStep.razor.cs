using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class SetupProcessSetupStep : ComponentBase, ISetupStep
{
    private bool _isMigrating = false;
    private bool _migrationSuccessful = false;
    private string? _errorMessage = null;

    public string Key { get; } = nameof(SetupProcessSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        // start migration process
        await StartMigrationAsync();
    }

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    private async Task StartMigrationAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _isMigrating = true;
        _migrationSuccessful = false;
        _errorMessage = null;
        await InvokeAsync(StateHasChanged);

        try
        {
            await MigrateDatabaseAsync(cancellationToken);

            _migrationSuccessful = true;
            await SetupService.SetStepStatusAsync(false, false, cancellationToken);
        }
        catch (HttpRequestException err)
        {
            // DE => Verbindung zum Server konnte nicht hergestellt werden. Stellen Sie sicher, dass der Server läuft und korrekt konfiguriert wurde und versuchen Sie es erneut.
            _errorMessage = "Unable to connect to the server. Make sure that the server is running and has been configured correctly, then try again.";
            await StepErrorAsync(cancellationToken);
        }
        catch (SetupCheckException err)
        {
            _errorMessage = err.Message;
            await StepErrorAsync(cancellationToken);
        }
        catch (Exception err)
        {
            _errorMessage = "error while migrating database: " + err.Message;
            await StepErrorAsync(cancellationToken);
        }
        finally
        {
            _isMigrating = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task MigrateDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await BackendClient.Setup.Start.PostAsync(new StartSetupRequest(),
                x =>
                {
                },
                cancellationToken);
        }
        catch (ApiException err) when (err.ResponseStatusCode == 400)
        {
            throw new SetupCheckException("Validation error for example with the database configuration, e.g. too short password, etc.");
        }
        catch (ApiException err) when (err.ResponseStatusCode == 422)
        {
            throw new SetupCheckException("Licenses not accepted");
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            throw new SetupCheckException("Unknown error while starting setup");
        }
        catch (Exception err)
        {
            throw new SetupCheckException(err.Message);
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

    private async Task OnCountdownFinishedAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        if (_migrationSuccessful)
        {
            await StepSuccessAsync(cancellationToken);
            await InvokeAsync(StateHasChanged);
        }
    }
}
