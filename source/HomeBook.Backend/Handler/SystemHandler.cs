using HomeBook.Backend.Responses;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public static class SystemHandler
{
    public static IResult HandleGetSystemInfo([FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            string dotnetVersion = Environment.Version.ToString();
            string appVersion = configuration["Version"] ?? "Unknown";
            string databaseProvider = configuration["Database:Provider"] ?? "Unknown";
            string deploymentType = "Docker";

            GetSystemInfoResponse response = new(dotnetVersion,
                appVersion,
                databaseProvider,
                deploymentType);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while retrieving system information.", statusCode: 500);
        }
    }

    public static async Task<IResult> HandleGetUsers([FromServices] IUserRepository userRepository,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Get all users from repository
            IEnumerable<User> allUsers = await userRepository.GetAllAsync(cancellationToken);
            int totalCount = allUsers.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Apply pagination
            IEnumerable<User> paginatedUsers = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            // Map to response models
            IEnumerable<UserResponse> userResponses = paginatedUsers.Select(user => new UserResponse(
                user.Id,
                user.Username,
                user.Created,
                user.Disabled,
                user.IsAdmin));

            GetUsersResponse response = new(userResponses, totalCount, page, pageSize, totalPages);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while retrieving users.", statusCode: 500);
        }
    }
}
