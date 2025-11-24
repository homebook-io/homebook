using HomeBook.Backend.Abstractions.Models.UserManagement;
using HomeBook.Backend.Module.Kitchen.Models;
using HomeBook.Backend.Module.Kitchen.Responses;

namespace HomeBook.Backend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeDto ToDto(this Data.Entities.Recipe recipe)
    {
        return new RecipeDto(
            recipe.Id,
            recipe.UserId,
            recipe.Name,
            recipe.NormalizedName,
            recipe.Description,
            recipe.DurationMinutes,
            recipe.CaloriesKcal,
            recipe.Servings);
    }

    public static async Task<RecipeResponse> ToResponseAsync(this RecipeDto recipe,
        Func<Guid, Task<UserInfo?>> getUserInfoAsync)
    {
        string? username = null;
        if (recipe.UserId.HasValue)
        {
            UserInfo? userInfo = await getUserInfoAsync(recipe.UserId.Value);
            username = userInfo?.Username;
        }

        return new RecipeResponse(recipe.Id,
            username,
            recipe.Name,
            recipe.NormalizedName,
            recipe.Description,
            recipe.DurationMinutes,
            recipe.CaloriesKcal,
            recipe.Servings);
    }
}
