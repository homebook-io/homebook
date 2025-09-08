using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;

namespace HomeBook.Backend.Endpoints;

public static class UpdateEndpoints
{
    public static IEndpointRouteBuilder MapUpdateEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/update")
            .WithTags("setup")
            .WithDescription("Endpoints for setup management");

        group.MapPost("/start", UpdateHandler.HandleStartUpdate)
            .WithName("StartUpdate")
            .WithDescription(new Description("start the update process",
                "HTTP 200: Update was successful",
                "HTTP 409: Setup was not executed yet - setup must be completed before update can be started",
                "HTTP 500: Unknown error while starting update"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces<string>(StatusCodes.Status500InternalServerError);


        return routeBuilder;
    }
}
