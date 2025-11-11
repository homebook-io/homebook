using HomeBook.Backend.DTOs.Responses.Kitchen;
using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;

namespace HomeBook.Backend.Endpoints;

public static class KitchenRecipeEndpoints
{
    public static IEndpointRouteBuilder MapKitchenRecipeEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/kitchen/recipes")
            .WithDescription("Endpoints to manage recipes informations")
            .RequireAuthorization();

        group.MapGet("/", KitchenRecipeHandler.HandleGetRecipes)
            .WithName("GetRecipes")
            .WithTags("Kitchen", "Recipes")
            .WithDescription(new Description(
                "returns recipes matching the search filter",
                "HTTP 200: Recipes were found",
                "HTTP 401: User is not authorized",
                "HTTP 500: Unknown error while getting recipes"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<RecipesListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", KitchenRecipeHandler.HandleCreateRecipe)
            .WithName("CreateRecipe")
            .WithTags("Kitchen", "Recipes")
            .WithDescription(new Description(
                "creates a new recipe",
                "HTTP 201: Recipe was created",
                "HTTP 400: Invalid request data",
                "HTTP 401: User is not authorized",
                "HTTP 500: Unknown error while creating recipe"))
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", KitchenRecipeHandler.HandleDeleteRecipe)
            .WithName("DeleteRecipe")
            .WithTags("Kitchen", "Recipes")
            .WithDescription(new Description(
                "deletes an existing recipe",
                "HTTP 200: Recipe was deleted",
                "HTTP 401: User is not authorized",
                "HTTP 404: Saving goal not found",
                "HTTP 500: Unknown error while deleting recipe"))
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
