namespace HomeBook.Frontend.Abstractions.Models;

public class PagedList<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IReadOnlyList<T> Items { get; set; }
}
