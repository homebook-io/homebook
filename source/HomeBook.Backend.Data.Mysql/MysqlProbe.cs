using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Setup;
using MySql.Data.MySqlClient;

namespace HomeBook.Backend.Data.Mysql;

/// <inheritdoc />
public class MysqlProbe : IDatabaseProbe
{
    /// <inheritdoc />
    public DatabaseProvider ProviderName { get; } = DatabaseProvider.MYSQL;

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
            string connectionString = $"Server={host};Port={port};Database={databaseName};Uid={username};Pwd={password};Connection Timeout=5;";

            await using MySqlConnection connection = new(connectionString);
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
