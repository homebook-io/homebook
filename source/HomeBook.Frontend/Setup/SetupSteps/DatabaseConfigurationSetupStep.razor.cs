using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Mappings;
using HomeBook.Frontend.Models.Setup;
using HomeBook.Frontend.Services.Exceptions;
using HomeBook.Frontend.Setup.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class DatabaseConfigurationSetupStep : ComponentBase, ISetupStep
{
    private bool _isChecking = false;
    private bool _databaseIsOk = false;
    private string? _errorMessage = null;
    public string Key { get; } = nameof(DatabaseConfigurationSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();
    private DatabaseConfigurationViewModel _databaseConfig = new();

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        try
        {
            CancellationToken cancellationToken = CancellationToken.None;

            DatabaseConfiguration? databaseConfiguration = await DatabaseSetupService
                .GetDatabaseConfigurationAsync(cancellationToken);
            _databaseConfig = databaseConfiguration?.ToViewModel() ?? new DatabaseConfigurationViewModel();
        }
        catch (InvalidConfigurationException err)
        {
            _errorMessage = err.Message;
        }
        catch (SetupException err)
        {
            _errorMessage = err.Message;
        }
        catch (Exception err)
        {
            _errorMessage = err.Message;
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task OnValidSubmit()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _isChecking = true;
        _databaseIsOk = false;
        _errorMessage = null;
        await InvokeAsync(StateHasChanged);

        bool checkSuccessful = false;
        try
        {
            await Task.WhenAll(
                Task.Delay(8000, cancellationToken),
                ConnectToDatabaseAsync(cancellationToken));

            _databaseIsOk = true;
            checkSuccessful = true;
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
            _errorMessage = "error while connecting to database: " + err.Message;
            await StepErrorAsync(cancellationToken);
        }
        finally
        {
            _isChecking = false;
            await InvokeAsync(StateHasChanged);
        }

        if (checkSuccessful)
        {
            await StepSuccessAsync(cancellationToken);
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ConnectToDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await BackendClient.Setup.Database.Check.PostAsync(
                new CheckDatabaseRequest
                {
                    DatabaseHost = _databaseConfig.Host,
                    DatabasePort = _databaseConfig.Port,
                    DatabaseName = _databaseConfig.DatabaseName,
                    DatabaseUserName = _databaseConfig.Username,
                    DatabaseUserPassword = _databaseConfig.Password
                },
                x =>
                {
                },
                cancellationToken);
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            throw new SetupCheckException("Unknown error while database connection check.");
        }
        catch (ApiException err) when (err.ResponseStatusCode == 503)
        {
            throw new SetupCheckException("Database is not available.");
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
        await SetupService.SetStepStatusAsync(false, false, cancellationToken);
        await Task.Delay(5000, cancellationToken);
        await SetupService.SetStepStatusAsync(true, false, cancellationToken);
    }
}
