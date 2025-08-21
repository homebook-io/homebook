using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class DatabaseMigrationSetupStep : ComponentBase, ISetupStep
{
    private bool _isMigrating = false;
    private bool _migrationSuccessful = false;
    private string? _errorMessage = null;

    public string Key { get; } = nameof(DatabaseMigrationSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    private async Task StartAsync()
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
            await BackendClient.Setup.Database.Migrate.PostAsync(x =>
                {
                },
                cancellationToken
            );
        }
        catch (ApiException err) when (err.ResponseStatusCode == 409)
        {
            throw new SetupCheckException("Database migration is already executed and not available.");
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            throw new SetupCheckException("Unknown error while database migration.");
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
