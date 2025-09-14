using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Setup;

namespace HomeBook.Backend.Data;

/// <inheritdoc />
public class DatabaseProviderResolver(IEnumerable<IDatabaseProbe> databaseProbes) : IDatabaseProviderResolver
{
    /// <inheritdoc />
    public async Task<DatabaseProvider?> ResolveAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Run all database probes in parallel
        IEnumerable<Task<DatabaseProvider?>> probeTasks = databaseProbes.Select(async probe =>
        {
            try
            {
                bool canConnect = await probe.CanConnectAsync(host, port, databaseName, username, password, cts.Token);
                return canConnect ? probe.ProviderName : (DatabaseProvider?)null;
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

        Task<DatabaseProvider?>[] taskArray = probeTasks.ToArray();

        // Wait for the first successful result
        while (taskArray.Length > 0)
        {
            Task<DatabaseProvider?> completedTask = await Task.WhenAny(taskArray);
            DatabaseProvider? result = await completedTask;

            if (result.HasValue)
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
