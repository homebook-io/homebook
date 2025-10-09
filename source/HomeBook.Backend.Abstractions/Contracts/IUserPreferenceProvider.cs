namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
///
/// </summary>
public interface IUserPreferenceProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetUserPreferredLocaleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="locale"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetUserPreferredLocaleAsync(Guid userId, string locale, CancellationToken cancellationToken);
}
