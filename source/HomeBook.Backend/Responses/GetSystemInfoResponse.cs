namespace HomeBook.Backend.Responses;

public record GetSystemInfoResponse(string DotnetRuntimeVersion, string AppVersion, string Database);
