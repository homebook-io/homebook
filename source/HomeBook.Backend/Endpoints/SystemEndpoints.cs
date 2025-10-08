using HomeBook.Backend.Handler;
using HomeBook.Backend.Responses;
using HomeBook.Backend.Middleware;

namespace HomeBook.Backend.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapSystemUsersEndpoints()
            .MapSystemInstanceEndpoints();

        return routeBuilder;
    }

    private static IEndpointRouteBuilder MapSystemUsersEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/system/users")
            .WithTags("system/users")
            .WithDescription("Endpoints for user management");

        group.MapGet("/", SystemHandler.HandleGetUsers)
            .WithName("GetUsers")
            .WithDescription("Returns all users with pagination, optionally filtered by username (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<UsersResponse>()
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", SystemHandler.HandleCreateUser)
            .WithName("CreateUser")
            .WithDescription("Creates a new user (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<CreateUserResponse>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{userId:guid}", SystemHandler.HandleGetUserById)
            .WithName("GetUserById")
            .WithDescription("Returns a user by its id (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<UserResponse>()
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{userId:guid}", SystemHandler.HandleDeleteUser)
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

        group.MapPut("/{userId:guid}/username", SystemHandler.HandleUpdateUsername)
            .WithName("UpdateUsername")
            .WithDescription("Updates a user's username (Admin only, checks for uniqueness ignoring case)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status409Conflict) // For username conflict
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{userId:guid}/password", SystemHandler.HandleUpdatePassword)
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

        group.MapPut("/{userId:guid}/admin", SystemHandler.HandleUpdateUserAdmin)
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

        group.MapPut("/{userId:guid}/enable", SystemHandler.HandleEnableUser)
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

        group.MapPut("/{userId:guid}/disable", SystemHandler.HandleDisableUser)
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

    private static IEndpointRouteBuilder MapSystemInstanceEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/system/instance")
            .WithTags("system/instance")
            .WithDescription("Endpoints for instance management");

        group.MapGet("/info", SystemHandler.HandleGetSystemInfo)
            .WithName("GetSystemInfo")
            .WithDescription("returns several system informations (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<GetSystemInfoResponse>()
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/name", SystemHandler.HandleUpdateInstanceName)
            .WithName("UpdateInstanceName")
            .WithDescription("Updates the instance name (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/default-locale", SystemHandler.HandleUpdateInstanceDefaultLocale)
            .WithName("UpdateInstanceDefaultLocale")
            .WithDescription("Updates the instance default locale (Admin only)")
            .WithMetadata(new RequireAdminAttribute())
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
