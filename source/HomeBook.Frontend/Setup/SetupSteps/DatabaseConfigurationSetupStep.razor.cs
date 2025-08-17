using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Mappings;
using HomeBook.Frontend.Models.Setup;
using HomeBook.Frontend.Services.Exceptions;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class DatabaseConfigurationSetupStep : ComponentBase, ISetupStep
{
    public string Key { get; } = nameof(DatabaseConfigurationSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();
    private DatabaseConfigurationViewModel _databaseConfig = new();
    private bool _isProcessing = false;
    private ConnectionResult? _connectionResult;

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
            // display error
        }
        catch (SetupException err)
        {
            // display error
        }
        catch (Exception ex)
        {
            // log error
            Console.Error.WriteLine($"Error during database configuration setup: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    public class ConnectionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    private async Task OnValidSubmit()
    {
        _isProcessing = true;
        _connectionResult = null;

        try
        {
            CancellationToken cancellationToken = CancellationToken.None;

            bool databaseConnected = await DatabaseSetupService.CheckConnectionAsync(
                _databaseConfig.Host,
                _databaseConfig.Port,
                _databaseConfig.DatabaseName,
                _databaseConfig.Username,
                _databaseConfig.Password,
                cancellationToken);
            int i = 0;
        }
        finally
        {
            _isProcessing = false;
        }
    }

    private void ClearForm()
    {
        _databaseConfig = new DatabaseConfigurationViewModel();
        _connectionResult = null;
    }
}
