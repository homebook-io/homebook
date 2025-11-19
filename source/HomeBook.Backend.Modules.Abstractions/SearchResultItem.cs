using HomeBook.Backend.Abstractions.Contracts;

namespace HomeBook.Backend.Modules.Abstractions;

public record SearchResult(
    int TotalCount,
    IEnumerable<ISearchResultItem> Items);

public record SearchResultItem(
    string Title,
    string? Description,
    string Url,
    string Icon,
    string Color) : ISearchResultItem;
