namespace HomeBook.Backend.Responses;

/// <summary>
/// Response model for instance information
/// </summary>
/// <param name="Name">The instance name</param>
/// <param name="DefaultLanguage">The default language of the instance</param>
public record GetInstanceInfoResponse(string Name, string DefaultLanguage);
