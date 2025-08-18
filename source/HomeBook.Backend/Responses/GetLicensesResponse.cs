namespace HomeBook.Backend.Responses;

public record GetLicensesResponse(License[] Licenses);

public record License(string Name, string MarkdownContent);
