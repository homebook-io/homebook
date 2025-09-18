namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines methods to manage the homebook instance
/// </summary>
public interface IInstanceManagementProvider
{
    /// <summary>
    /// returns the name of the homebook instance
    /// </summary>
    /// <returns></returns>
    Task<string> GetInstanceNameAsync(CancellationToken cancellationToken = default);
}
