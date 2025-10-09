namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines methods to get and set user preferences
/// </summary>
public interface IUserPreferencesProvider
{
    /// <summary>
    /// gets the current user locale
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetLocaleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// sets the current user locale
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetLocaleAsync(string locale, CancellationToken cancellationToken = default);
}
