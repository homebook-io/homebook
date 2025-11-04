using HomeBook.Backend.DTOs.Responses.Finances;
using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Endpoints;

public static class FinanceSavingGoalEndpoints
{
    public static IEndpointRouteBuilder MapFinancesSavingGoalEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/finances/saving-goals")
            .WithDescription("Endpoints for finances saving goals")
            .RequireAuthorization();

        group.MapGet("/", FinanceSavingGoalHandler.HandleGetSavingGoals)
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
            .Produces<FinanceSavingGoalListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", FinanceSavingGoalHandler.HandleCreateSavingGoal)
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

        group.MapPatch("/{savingGoalId:guid}/name", FinanceSavingGoalHandler.HandleUpdateSavingGoalName)
            .WithName("UpdateSavingGoalName")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "updates the name of an existing finances saving goal for the user",
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

        group.MapPatch("/{savingGoalId:guid}/appearance", FinanceSavingGoalHandler.HandleUpdateSavingGoalAppearance)
            .WithName("UpdateSavingGoalAppearance")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "updates the appearance of an existing finances saving goal for the user",
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

        group.MapPatch("/{savingGoalId:guid}/amounts", FinanceSavingGoalHandler.HandleUpdateSavingGoalAmounts)
            .WithName("UpdateSavingGoalAmounts")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "updates the amounts of an existing finances saving goal for the user",
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

        group.MapPatch("/{savingGoalId:guid}/info", FinanceSavingGoalHandler.HandleUpdateSavingGoalInfo)
            .WithName("UpdateSavingGoalInfo")
            .WithTags("Finances", "SavingGoals")
            .WithDescription(new Description(
                "updates the info (target date, etc.) of an existing finances saving goal for the user",
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

        group.MapDelete("/{savingGoalId:guid}", FinanceSavingGoalHandler.HandleDeleteSavingGoal)
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
