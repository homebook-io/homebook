using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Data.Sqlite;

namespace HomeBook.Backend.Data.Sqlite;

public class SqliteProbe : IDatabaseProbe
{
    /// <inheritdoc />
    public string ProviderName { get; } = "SQLITE";

    /// <inheritdoc />
    public Task<bool> CanConnectAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> CanConnectAsync(string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string connectionString = ConnectionStringBuilder.Build(filePath);

            await using SqliteConnection connection = new(connectionString);
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
