using HomeBook.Backend.Abstractions;
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
        // Run all database probes in parallel
        IEnumerable<Task<DatabaseProvider?>> probeTasks = databaseProbes.Select(async probe =>
        {
            try
            {
                bool canConnect = await probe.CanConnectAsync(host, port, databaseName, username, password, cancellationToken);
                return canConnect ? probe.ProviderName : (DatabaseProvider?)null;
            }
            catch
            {
                // If a probe fails, it's not a valid provider for this connection
                return null;
            }
        });

        DatabaseProvider?[] results = await Task.WhenAll(probeTasks);

        // Return the first successful provider
        return results.FirstOrDefault(result => result.HasValue);
    }
}
