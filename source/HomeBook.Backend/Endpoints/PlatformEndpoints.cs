using HomeBook.Backend.Handler;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Endpoints;

public static class PlatformEndpoints
{
    public static IEndpointRouteBuilder MapPlatformEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/platform")
            .WithTags("platform")
            .WithDescription("Endpoints for platform management");

        group.MapGet("/locales", PlatformHandler.HandleGetLocales)
            .WithName("GetLocales")
            .WithDescription("returns all available locales")
            .WithOpenApi()
            .Produces<GetLocalesResponse>( StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
