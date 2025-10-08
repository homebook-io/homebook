using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Enums;

namespace HomeBook.Frontend.Services;

/// <inheritdoc />
public class StartupService(
    IServiceProvider serviceProvider,
    IInstanceManagementProvider InstanceManagementProvider,
    IJsLocalStorageProvider JsLocalStorageProvider,
    ILogger<StartupService> logger) : IStartupService
{
    /// <inheritdoc />
    public async Task StartAsync(InstanceStatus instanceStatus,
        CancellationToken cancellationToken)
    {
        if (instanceStatus == InstanceStatus.Running)
        {
            // load instance data into cache
            await LoadInstanceDataAsync(cancellationToken);
        }
    }

    private async Task LoadInstanceDataAsync(CancellationToken cancellationToken)
    {
        string instanceName = await InstanceManagementProvider.GetInstanceNameAsync(cancellationToken);
        await JsLocalStorageProvider.SetItemAsync(JsLocalStorageKeys.HomeBookInstanceName,
            instanceName,
            cancellationToken);
    }
}
