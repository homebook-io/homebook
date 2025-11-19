using HomeBook.Backend.Modules.Abstractions;

namespace HomeBook.Backend.Abstractions.Contracts;

public interface ISearchProvider
{
    Task<IEnumerable<ISearchAggregationResult>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
