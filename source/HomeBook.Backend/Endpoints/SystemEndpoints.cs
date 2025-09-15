using HomeBook.Backend.Handler;
using HomeBook.Backend.Responses;
using HomeBook.Backend.Middleware;

namespace HomeBook.Backend.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/system")
            .WithTags("system")
            .WithDescription("Endpoints for system informations");

        group.MapGet("/", SystemHandler.HandleGetSystemInfo)
            .WithName("GetSystemInfo")
            .WithDescription("returns several system informations")
            .WithOpenApi()
            .Produces<GetSystemInfoResponse>()
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/users", SystemHandler.HandleGetUsers)
            .WithName("GetUsers")
            .WithDescription("Returns all users with pagination (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<GetUsersResponse>()
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/users", SystemHandler.HandleCreateUser)
            .WithName("CreateUser")
            .WithDescription("Creates a new user (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<CreateUserResponse>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/users", SystemHandler.HandleDeleteUser)
            .WithName("DeleteUser")
            .WithDescription("Deletes a user (Admin only, cannot delete self)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/users/password", SystemHandler.HandleUpdatePassword)
            .WithName("UpdateUserPassword")
            .WithDescription("Updates a user's password (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/users/admin", SystemHandler.HandleUpdateUserAdmin)
            .WithName("UpdateUserAdmin")
            .WithDescription("Updates a user's admin status (Admin only, cannot change own status)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/users/{userId:guid}/enable", SystemHandler.HandleEnableUser)
            .WithName("EnableUser")
            .WithDescription("Enables a disabled user (Admin only, cannot enable self)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/users/{userId:guid}/disable", SystemHandler.HandleDisableUser)
            .WithName("DisableUser")
            .WithDescription("Disables an active user (Admin only, cannot disable self)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
