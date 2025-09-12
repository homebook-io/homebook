using HomeBook.Backend.Handler;
using HomeBook.Backend.Responses;

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
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<GetSystemInfoResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
