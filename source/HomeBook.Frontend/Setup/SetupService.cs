using System.Net;
using HomeBook.Client;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Setup.SetupSteps;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Setup;

public class SetupService(
    IRequestAdapter requestAdapter,
    BackendClient backendClient) : ISetupService
{
    public Guid Id { get; } = Guid.NewGuid();
    private bool _isDone = false;
    private Dictionary<string, object> _storage = new();
    private List<ISetupStep> _setupSteps = [];
    public Func<Task>? OnSetupSuccessful { get; set; }
    public Func<ISetupStep, Task>? OnStepSuccessful { get; set; }
    public Func<ISetupStep, bool, Task>? OnStepFailed { get; set; }
    public Func<Task>? OnSetupStepsInitialized { get; set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        List<ISetupStep> setupSteps = [];

        setupSteps.Add(new BackendConnectionSetupStep());
        setupSteps.Add(new LicenseAgreementSetupStep());
        setupSteps.Add(new DatabaseConfigurationSetupStep());
        setupSteps.Add(new AdminUserSetupStep());
        setupSteps.Add(new ConfigurationSetupStep());
        setupSteps.Add(new SetupProcessSetupStep());

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
        try
        {
            NativeResponseHandler native = new();
            await backendClient.Setup.Availability.GetAsync(cfg =>
            {
                cfg.Options.Add(new ResponseHandlerOption
                {
                    ResponseHandler = native
                });
            }, cancellationToken);
            HttpStatusCode? status = (native.Value as HttpResponseMessage)?.StatusCode;

            return status switch
            {
                HttpStatusCode.OK => false, // setup can be started
                HttpStatusCode.Created => false, // setup is done, but an update is required
                HttpStatusCode.NoContent => true, // setup is finished and no update is required => Homebook is ready to use
                HttpStatusCode.Conflict => false, // setup is not available (already running)
                _ => false // unknown error
            };
        }
        catch (ApiException err)
        {
            // unknown error
            return false;
        }
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

    public async Task TriggerSetupFinishedAsync(CancellationToken cancellationToken = default)
    {
        if (OnSetupSuccessful != null)
            await OnSetupSuccessful.Invoke();
    }
}
