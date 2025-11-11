using HomeBook.Backend.DTOs.Responses.Finances;
using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;

namespace HomeBook.Backend.Endpoints;

public static class FinanceCalculationsEndpoints
{
    public static IEndpointRouteBuilder MapFinancesCalculationEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/finances/calculations")
            .WithDescription("Endpoints for finances calculations")
            .RequireAuthorization();

        group.MapPost("/savings", FinanceCalculationsHandler.HandleCalculateSavings)
            .WithName("CalculateSavings")
            .WithTags("Finances", "Savings", "Calculate")
            .WithDescription(new Description(
                "returns the calculated savings based on the provided parameters",
                "HTTP 200: successfully calculated savings",
                "HTTP 401: User is not authorized",
                "HTTP 500: Unknown error while getting saving goals"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<CalculatedSavingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
