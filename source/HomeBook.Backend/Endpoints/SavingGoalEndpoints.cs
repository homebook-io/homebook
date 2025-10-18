using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Endpoints;

public static class SavingGoalEndpoints
{
    public static IEndpointRouteBuilder MapFinancesSavingGoal(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/finances/saving-goals")
            .WithDescription("Endpoints for finances saving goals")
            .RequireAuthorization();

        group.MapGet("/", SavingGoalHandler.HandleGetSavingGoals)
            .WithName("GetSavingGoals")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "returns the users finances saving goals",
                "HTTP 200: Finances saving goals were found",
                "HTTP 401: User is not authorized",
                "HTTP 500: Unknown error while getting saving goals"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<GetFinanceSavingGoalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
