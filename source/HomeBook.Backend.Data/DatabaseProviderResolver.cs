using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Setup;

namespace HomeBook.Backend.Data;

/// <inheritdoc />
public class DatabaseProviderResolver(IEnumerable<IDatabaseProbe> databaseProbes) : IDatabaseProviderResolver
{
    /// <inheritdoc />
    public async Task<string?> ResolveAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Run all database probes in parallel
        IEnumerable<Task<string?>> probeTasks = databaseProbes.Select(async probe =>
        {
            try
            {
                bool canConnect = await probe.CanConnectAsync(host, port, databaseName, username, password, cts.Token);
                return canConnect ? probe.ProviderName : (string?)null;
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled, return null
                return null;
            }
            catch
            {
                // If a probe fails, it's not a valid provider for this connection
                return null;
            }
        });

        Task<string?>[] taskArray = probeTasks.ToArray();

        // Wait for the first successful result
        while (taskArray.Length > 0)
        {
            Task<string?> completedTask = await Task.WhenAny(taskArray);
            string? result = await completedTask;

            if (!string.IsNullOrEmpty(result))
            {
                // Cancel all remaining tasks since we found a valid provider
                await cts.CancelAsync();
                return result;
            }

            // Remove the completed task and continue with remaining tasks
            taskArray = taskArray.Where(t => t != completedTask).ToArray();
        }

        // No successful provider found
        return null;
    }
}
