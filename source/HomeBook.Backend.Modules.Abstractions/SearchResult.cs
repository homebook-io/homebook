namespace HomeBook.Backend.Modules.Abstractions;

public record SearchResult(
    int TotalCount,
    IEnumerable<SearchResultItem> Items);

public record SearchResultItem(
    string Title,
    string? Description,
    string Url,
    string Icon,
    string Color);
