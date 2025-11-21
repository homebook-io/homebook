using System.Security.Claims;
using HomeBook.Backend.Core.Modules.Utilities;
using HomeBook.Backend.Module.Kitchen.Contracts;
using HomeBook.Backend.Module.Kitchen.Mappings;
using HomeBook.Backend.Module.Kitchen.Models;
using HomeBook.Backend.Module.Kitchen.Requests;
using HomeBook.Backend.Module.Kitchen.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Module.Kitchen.Handler;

public class KitchenRecipeHandler
{
    /// <summary>
    /// returns recipes matching the search filter
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetRecipes(string searchFilter,
        [FromServices] ILogger<KitchenRecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            RecipeDto[] recipeDtos = await recipesProvider.GetRecipesAsync(searchFilter,
                cancellationToken);
            RecipeResponse[] recipes = recipeDtos
                .Select(sg => sg.ToResponse())
                .ToArray();

            return TypedResults.Ok(new RecipesListResponse(recipes));
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while getting recipes");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleCreateRecipe(ClaimsPrincipal user,
        [FromBody] CreateRecipeRequest request,
        [FromServices] ILogger<KitchenRecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            Guid createdId = await recipesProvider.CreateAsync(request.Name,
                request.Description,
                request.Duration,
                request.CaloriesKcal,
                request.Servings,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while creating recipe");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleDeleteRecipe(Guid id,
        ClaimsPrincipal user,
        [FromServices] ILogger<KitchenRecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await recipesProvider.DeleteAsync(id,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while deleting recipe for {Id}",
                id);
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleUpdateRecipeName(Guid id,
        ClaimsPrincipal user,
        [FromBody] UpdateRecipeNameRequest request,
        [FromServices] ILogger<KitchenRecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await recipesProvider.UpdateNameAsync(id,
                request.Name,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while updating recipe name for {Id}",
                id);
            return TypedResults.InternalServerError(err.Message);
        }
    }
}
