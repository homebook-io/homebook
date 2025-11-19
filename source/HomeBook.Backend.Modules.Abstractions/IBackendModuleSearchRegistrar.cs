namespace HomeBook.Backend.Modules.Abstractions;

public interface IBackendModuleSearchRegistrar
{
    /// <summary>
    /// the display name of this module
    /// </summary>
    string Name { get; }

    /// <summary>
    /// the key of this module (used for endpoint grouping, etc.)
    /// </summary>
    string Key { get; }

    /// <summary>
    /// the author of this module
    /// </summary>
    string Author { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SearchResult> SearchAsync(string query,
        CancellationToken cancellationToken = default);
}
