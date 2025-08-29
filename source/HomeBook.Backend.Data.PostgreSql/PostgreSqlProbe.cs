using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Setup;
using Npgsql;

namespace HomeBook.Backend.Data.PostgreSql;

/// <inheritdoc />
public class PostgreSqlProbe : IDatabaseProbe
{
    /// <inheritdoc />
    public DatabaseProvider ProviderName { get; } = DatabaseProvider.POSTGRESQL;

    /// <inheritdoc />
    public async Task<bool> CanConnectAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string connectionString = $"Host={host};Port={port};Database={databaseName};Username={username};Password={password};Timeout=5;";

            await using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync(cancellationToken);

            bool isConnected = connection.State == System.Data.ConnectionState.Open;
            if (isConnected)
            {
                await connection.CloseAsync();
            }

            return isConnected;
        }
        catch
        {
            return false;
        }
    }
}
