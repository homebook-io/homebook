namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
///
/// </summary>
public interface IDatabaseSetupService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="databaseName"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CheckConnectionAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken);
}
