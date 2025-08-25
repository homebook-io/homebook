using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class SetupProcessSetupStep : ComponentBase, ISetupStep
{
    private bool _setupIsRunning = false;
    private bool _setupSuccessful = false;
    private string? _errorMessage = null;

    public string Key { get; } = nameof(SetupProcessSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();
    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;
    }

    private async Task StartSetupAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _setupIsRunning = true;
        _setupSuccessful = false;
        _errorMessage = null;
        await InvokeAsync(StateHasChanged);

        try
        {
            await StartSetupAsync(cancellationToken);

            _setupSuccessful = true;
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
            _errorMessage = "error while setup: " + err.Message;
            await StepErrorAsync(cancellationToken);
        }
        finally
        {
            _setupIsRunning = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task StartSetupAsync(CancellationToken cancellationToken)
    {
        try
        {
            StartSetupRequest request = new();

            string? homebookUserName = await SetupService.GetStorageValueAsync<string>("HOMEBOOK_USERNAME", cancellationToken);
            if (!string.IsNullOrEmpty(homebookUserName))
                request.HomebookUserName = homebookUserName;

            string? homebookUserPassword = await SetupService.GetStorageValueAsync<string>("HOMEBOOK_PASSWORD", cancellationToken);
            if (!string.IsNullOrEmpty(homebookUserPassword))
                request.HomebookUserPassword = homebookUserPassword;

            string? databaseType = await SetupService.GetStorageValueAsync<string>("DATABASE_TYPE", cancellationToken);
            if (!string.IsNullOrEmpty(databaseType))
                request.DatabaseType = databaseType;

            string? databaseHost = await SetupService.GetStorageValueAsync<string>("DATABASE_HOST", cancellationToken);
            if (!string.IsNullOrEmpty(databaseHost))
                request.DatabaseHost = databaseHost;

            ushort? databasePort = await SetupService.GetStorageValueAsync<ushort>("DATABASE_PORT", cancellationToken);
            if (databasePort > 0)
                request.DatabasePort = databasePort;

            string? databaseName = await SetupService.GetStorageValueAsync<string>("DATABASE_NAME", cancellationToken);
            if (!string.IsNullOrEmpty(databaseName))
                request.DatabaseName = databaseName;

            string? databaseUsername = await SetupService.GetStorageValueAsync<string>("DATABASE_USERNAME", cancellationToken);
            if (!string.IsNullOrEmpty(databaseUsername))
                request.DatabaseUserName = databaseUsername;

            string? databasePassword = await SetupService.GetStorageValueAsync<string>("DATABASE_PASSWORD", cancellationToken);
            if (!string.IsNullOrEmpty(databasePassword))
                request.DatabaseUserPassword = databasePassword;

            await BackendClient.Setup.Start.PostAsync(request,
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

    private async Task OnStartSetupCountdownFinishedAsync()
    {
        // start setup process
        await StartSetupAsync();
    }

    private async Task OnCountdownFinishedAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        if (_setupSuccessful)
        {
            await StepSuccessAsync(cancellationToken);
            await InvokeAsync(StateHasChanged);
        }
    }
}
