using System.Globalization;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Enums;

namespace HomeBook.Frontend.Services;

/// <inheritdoc />
public class StartupService(
    IServiceProvider serviceProvider,
    IInstanceManagementProvider instanceManagementProvider,
    IJsLocalStorageProvider jsLocalStorageProvider,
    ILocalizationService localizationService,
    ISetupService setupService,
    ILogger<StartupService> logger) : IStartupService
{
    public event Action<AppStatus>? ApplicationInitialized;

    public AppStatus Status { get; private set; } = AppStatus.Initializing;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application");

        // 1. wait for backend to be ready
        await setupService.InitializeAsync(cancellationToken);
        await WaitForServerAndGetSetupStatusAsync(cancellationToken);

        if (Status == AppStatus.Error)
        {
            // TODO: show error page with message that app could not started
            return;
        }

        // 2. startup application
        if (Status == AppStatus.Running)
        {
            // load instance data into cache
            await LoadInstanceDataAsync(cancellationToken);
        }

        await InitializeLocalizing(cancellationToken);

        // Notify authentication state changed
        ApplicationInitialized?.Invoke(Status);
    }

    private async Task InitializeLocalizing(CancellationToken cancellationToken)
    {
        await localizationService.InitializeAsync(cancellationToken);

        CultureInfo culture = await localizationService.GetCultureAsync(cancellationToken);
        await localizationService.SetCultureAsync(culture,
            false,
            cancellationToken);
    }

    private async Task WaitForServerAndGetSetupStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            Status = (await setupService.GetInstanceStatusAsync(cancellationToken)) ?? AppStatus.Error;
        }
        catch (HttpRequestException err) when (err.Message.ToLowerInvariant().Contains("failed to fetch"))
        {
            await WaitForServerAndGetSetupStatusAsync(cancellationToken);
        }
        catch (Exception)
        {
            logger.LogError("Failed to get instance status");
        }
    }

    private async Task LoadInstanceDataAsync(CancellationToken cancellationToken)
    {
        string instanceName = await instanceManagementProvider.GetInstanceNameAsync(cancellationToken);
        await jsLocalStorageProvider.SetItemAsync(JsLocalStorageKeys.HomeBookInstanceName,
            instanceName,
            cancellationToken);
    }
}
