using Microsoft.Extensions.Hosting;

namespace HomeBook.UnitTests.TestCore;

public class TestHostApplicationLifetime : IHostApplicationLifetime
{
    private readonly CancellationTokenSource _applicationStartedTokenSource = new();
    private readonly CancellationTokenSource _applicationStoppingTokenSource = new();
    private readonly CancellationTokenSource _applicationStoppedTokenSource = new();

    public void StopApplication()
    {
        if (!_applicationStoppingTokenSource.IsCancellationRequested)
        {
            _applicationStoppingTokenSource.Cancel();
        }

        if (!_applicationStoppedTokenSource.IsCancellationRequested)
        {
            _applicationStoppedTokenSource.Cancel();
        }
    }

    public CancellationToken ApplicationStarted => _applicationStartedTokenSource.Token;
    public CancellationToken ApplicationStopped => _applicationStoppedTokenSource.Token;
    public CancellationToken ApplicationStopping => _applicationStoppingTokenSource.Token;

    /// <summary>
    /// Simulates the application start by canceling the ApplicationStarted token
    /// </summary>
    public void SimulateApplicationStarted()
    {
        if (!_applicationStartedTokenSource.IsCancellationRequested)
        {
            _applicationStartedTokenSource.Cancel();
        }
    }
}
