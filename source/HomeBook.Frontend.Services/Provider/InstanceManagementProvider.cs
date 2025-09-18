using HomeBook.Client;
using HomeBook.Frontend.Abstractions.Contracts;

namespace HomeBook.Frontend.Services.Provider;

public class InstanceManagementProvider(BackendClient backendClient) : IInstanceManagementProvider
{
    private string? _instanceName;

    public async Task<string> GetInstanceNameAsync(CancellationToken cancellationToken = default)
    {
        if (_instanceName is null)
            await LoadInstanceInfoAsync(cancellationToken);

        return _instanceName ?? string.Empty;
    }

    private async Task LoadInstanceInfoAsync(CancellationToken cancellationToken)
    {
        string? instanceName = await backendClient.Info.Name.GetAsync(x =>
            {
            },
            cancellationToken);

        _instanceName = instanceName;
    }
}
