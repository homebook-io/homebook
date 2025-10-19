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

        group.MapPost("/", SavingGoalHandler.HandleCreateSavingGoal)
            .WithName("CreateSavingGoal")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "creates a new finances saving goal for the user",
                "HTTP 201: Finances saving goal was created",
                "HTTP 400: Invalid request data",
                "HTTP 401: User is not authorized",
                "HTTP 500: Unknown error while creating saving goal"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{savingGoalId:guid}", SavingGoalHandler.HandleUpdateSavingGoal)
            .WithName("UpdateSavingGoal")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "updates an existing finances saving goal for the user",
                "HTTP 200: Finances saving goal was updated",
                "HTTP 400: Invalid request data",
                "HTTP 401: User is not authorized",
                "HTTP 404: Saving goal not found",
                "HTTP 500: Unknown error while updating saving goal"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{savingGoalId:guid}", SavingGoalHandler.HandleDeleteSavingGoal)
            .WithName("DeleteSavingGoal")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "deletes an existing finances saving goal for the user",
                "HTTP 200: Finances saving goal was deleted",
                "HTTP 401: User is not authorized",
                "HTTP 404: Saving goal not found",
                "HTTP 500: Unknown error while deleting saving goal"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
