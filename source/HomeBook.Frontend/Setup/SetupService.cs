using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.SetupSteps;

namespace HomeBook.Frontend.Setup;

public class SetupService(BackendClient backendClient) : ISetupService
{
    public Guid Id { get; } = Guid.NewGuid();
    private bool _isDone = false;
    private Dictionary<string, object> _storage = new();
    private List<ISetupStep> _setupSteps = [];
    public Func<ISetupStep, Task>? OnStepSuccessful { get; set; }
    public Func<ISetupStep, bool, Task>? OnStepFailed { get; set; }
    public Func<Task>? OnSetupStepsInitialized { get; set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        List<ISetupStep> setupSteps = [];

        setupSteps.Add(new BackendConnectionSetupStep());
        setupSteps.Add(new LicenseAgreementSetupStep());
        setupSteps.Add(new DatabaseConfigurationSetupStep());
        setupSteps.Add(new SetupProcessSetupStep());
        setupSteps.Add(new AdminUserSetupStep());
        setupSteps.Add(new ConfigurationSetupStep());

        _setupSteps = setupSteps;
    }

    public async Task TriggerOnMudStepInitialized(CancellationToken cancellationToken = default)
    {
        if (OnSetupStepsInitialized is not null)
            await OnSetupStepsInitialized.Invoke();
    }

    public async Task<ISetupStep[]> GetSetupStepsAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return _setupSteps.ToArray();
    }

    public async Task<ISetupStep?> GetActiveSetupStepAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        ISetupStep? activeStep = _setupSteps.FirstOrDefault(step => !step.IsSuccessful);

        return activeStep;
    }

    public async Task<bool> IsSetupDoneAsync(CancellationToken cancellationToken = default)
    {
        bool isSetupDone = _setupSteps.All(step => step.IsSuccessful);

        return isSetupDone;
    }

    public async Task SetStepStatusAsync(bool success,
        bool hasError,
        CancellationToken cancellationToken = default)
    {
        ISetupStep? activeStep = await GetActiveSetupStepAsync(cancellationToken);
        if (activeStep is null)
            throw new InvalidOperationException("No active setup step found.");

        if (hasError)
        {
            // set as failed
            activeStep.HasError = true;

            if (OnStepFailed != null)
                await OnStepFailed.Invoke(activeStep, true);
            return;
        }

        // reset failed status
        activeStep.HasError = false;
        if (OnStepFailed != null)
            await OnStepFailed.Invoke(activeStep, false);

        if (success)
        {
            // set as successful
            activeStep.IsSuccessful = true;
            if (OnStepSuccessful != null)
                await OnStepSuccessful.Invoke(activeStep);
        }
    }

    public Task SetStorageValueAsync(string key, object value, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        _storage[key] = value;
        return Task.CompletedTask;
    }

    public Task<T?> GetStorageValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        if (_storage.TryGetValue(key, out object? value) && value is T typedValue)
        {
            return Task.FromResult(typedValue)!;
        }

        return Task.FromResult<T?>(default);
    }
}
