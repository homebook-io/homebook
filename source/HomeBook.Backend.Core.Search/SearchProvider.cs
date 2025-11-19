using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Search.Models;
using HomeBook.Backend.Modules.Abstractions;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Core.Search;

public class SearchProvider(
    ILogger<SearchProvider> logger,
    IEnumerable<IBackendModuleSearchRegistrar> modules) : ISearchProvider
{
    public async Task<IEnumerable<ISearchAggregationResult>> SearchAsync(string query,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Task<SearchResult>> moduleSearchTasks = modules
            .Select(async module =>
            {
                try
                {
                    logger.LogDebug("Requesting module {Module} for search query '{Query}'",
                        module.Name,
                        query);

                    SearchResult result = await module.SearchAsync(query, cancellationToken);

                    logger.LogDebug("Module {Module} returned search result with {Count} items for query '{Query}'",
                        module.Name,
                        result.Items.Count(),
                        query);

                    return result;
                }
                catch (OperationCanceledException)
                {
                    // Task was cancelled, return null
                    return null;
                }
                catch (Exception err)
                {
                    logger.LogError(err,
                        "Error while requesting module {Module} for search query '{Query}'",
                        module.Name,
                        query);

                    return null;
                }
            })
            .Where(result => result is not null)!;

        IEnumerable<SearchResult> searchResults = await Task.WhenAll(moduleSearchTasks.ToArray());
        var blub = new List<SearchAggregationResult>();
        return blub;
    }
}
