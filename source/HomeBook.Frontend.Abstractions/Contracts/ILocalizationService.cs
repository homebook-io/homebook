using System.Globalization;

namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines localization service
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// initializes localization service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CultureInfo> GetCultureAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="forceLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetCultureAsync(string culture,
        bool forceLoad = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <param name="forceLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetCultureAsync(CultureInfo cultureInfo,
        bool forceLoad = true,
        CancellationToken cancellationToken = default);
}
