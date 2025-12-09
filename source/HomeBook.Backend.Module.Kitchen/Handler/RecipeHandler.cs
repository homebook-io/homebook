using System.Security.Claims;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Modules.Utilities;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Module.Kitchen.Contracts;
using HomeBook.Backend.Module.Kitchen.Mappings;
using HomeBook.Backend.Module.Kitchen.Models;
using HomeBook.Backend.Module.Kitchen.Requests;
using HomeBook.Backend.Module.Kitchen.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Module.Kitchen.Handler;

public class RecipeHandler
{
    /// <summary>
    /// returns recipes matching the search filter
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="userProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetRecipes(string searchFilter,
        [FromServices] ILogger<RecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        [FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            RecipeDto[] recipeDtos = await recipesProvider.GetRecipesAsync(searchFilter,
                cancellationToken);

            List<RecipeResponse> recipes = [];
            foreach (RecipeDto recipeDto in recipeDtos)
            {
                RecipeResponse recipeResponse = await recipeDto.ToResponseAsync(async userId =>
                    await userProvider.GetUserByIdAsync(userId, cancellationToken)
                );
                recipes.Add(recipeResponse);
            }

            return TypedResults.Ok(new RecipesListResponse(recipes.ToArray()));
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while getting recipes");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// returns recipe by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="logger"></param>
    /// <param name="recipesProvider"></param>
    /// <param name="userProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetRecipeById(Guid id,
        [FromServices] ILogger<RecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        [FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            RecipeDto? recipeDto = await recipesProvider.GetRecipeByIdAsync(id,
                cancellationToken);

            if (recipeDto is null)
                return TypedResults.NotFound();

            RecipeDetailResponse response = await recipeDto.ToDetailResponseAsync(async userId =>
                await userProvider.GetUserByIdAsync(userId, cancellationToken)
            );
            return TypedResults.Ok(response);
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
        [FromServices] ILogger<RecipeHandler> logger,
        [FromServices] IRecipesProvider recipesProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            Guid createdRecipeId = await recipesProvider.CreateAsync(request.Name,
                userId,
                request.Description,
                request.Servings,
                request.DurationWorkingMinutes,
                request.DurationCookingMinutes,
                request.DurationRestingMinutes,
                request.CaloriesKcal,
                request.Comments,
                request.Source,
                cancellationToken);

            if (request.Ingredients is not null
                && request.Ingredients.Length != 0)
                foreach (CreateRecipeIngredientRequest ingredient in request.Ingredients)
                {
                    await recipesProvider.AddIngredientToRecipeAsync(createdRecipeId,
                        ingredient.Name,
                        ingredient.Quantity,
                        ingredient.Unit,
                        cancellationToken);
                }

            if (request.Steps is not null
                && request.Steps.Length != 0)
                foreach (CreateRecipeStepRequest si in request.Steps)
                {
                    await recipesProvider.AddStepToRecipeAsync(createdRecipeId,
                        si.Position,
                        si.Description,
                        si.TimerDurationInSeconds,
                        cancellationToken);
                }

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
        [FromServices] ILogger<RecipeHandler> logger,
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
        [FromServices] ILogger<RecipeHandler> logger,
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
