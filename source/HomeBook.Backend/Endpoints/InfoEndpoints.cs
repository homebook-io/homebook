using HomeBook.Backend.Handler;
using HomeBook.Backend.Responses;
using HomeBook.Backend.Middleware;

namespace HomeBook.Backend.Endpoints;

public static class InfoEndpoints
{
    public static IEndpointRouteBuilder MapInfoEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/info")
            .WithTags("info")
            .WithDescription("Endpoints for instance information");

        group.MapGet("/", InfoHandler.HandleGetInstanceInfo)
            .WithName("GetInstanceInfo")
            .WithDescription("Returns instance information (requires authentication)")
            .RequireAuthorization()
            .WithOpenApi()
            .Produces<GetInstanceInfoResponse>()
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/name", InfoHandler.HandleGetInstanceName)
            .WithName("GetInstanceName")
            .WithDescription("Returns the instance name as a string (public endpoint)")
            .AllowAnonymous()
            .WithOpenApi()
            .Produces<string>()
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
