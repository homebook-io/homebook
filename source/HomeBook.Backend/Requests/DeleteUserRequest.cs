namespace HomeBook.Backend.Requests;

/// <summary>
/// Request model for deleting a user
/// </summary>
/// <param name="UserId">The ID of the user to delete</param>
public record DeleteUserRequest(Guid UserId);
