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

        return routeBuilder;
    }
}
