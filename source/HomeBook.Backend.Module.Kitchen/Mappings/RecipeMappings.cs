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
            recipe.Servings,
            recipe.DurationWorkingMinutes,
            recipe.DurationCookingMinutes,
            recipe.DurationRestingMinutes,
            recipe.CaloriesKcal,
            recipe.Comments,
            recipe.Source);
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
            recipe.DurationWorkingMinutes,
            recipe.CaloriesKcal,
            recipe.Servings);
    }

    public static async Task<RecipeDetailResponse> ToDetailResponseAsync(this RecipeDto recipe,
        Func<Guid, Task<UserInfo?>> getUserInfoAsync)
    {
        string? username = null;
        if (recipe.UserId.HasValue)
        {
            UserInfo? userInfo = await getUserInfoAsync(recipe.UserId.Value);
            username = userInfo?.Username;
        }

        var ingredients = new List<IngredientResponse>
        {
            new(Guid.NewGuid(),
                1,
                "Scheibe",
                "Schinken",
                "gekochter, oder anderer Belag, z.B. Putenbrust oder Salami"),
            new(Guid.NewGuid(), 1, null, "Ei", "gekocht"),
            new(Guid.NewGuid(), 1, "Scheibe", "Käse", "am besten Emmentaler , Ihr könnt aber auch anderen nehmen"),
            new(Guid.NewGuid(), 1, null, "Salatblatt", null),
            new(Guid.NewGuid(), 0.5, null, "Tomate", null),
            new(Guid.NewGuid(), null, null, "Salz und Pfeffer, Grillgewürz", null),
            new(Guid.NewGuid(), 3, "EL", "Mayonaise", null),
            new(Guid.NewGuid(), 2, "Scheiben", "Sandwich Toast", null),
            new(Guid.NewGuid(), 4, "x", "Zahnstocher", "o.ä. zum Fixieren")
        };

        var steps = new List<StepResponse>
        {
            new(Guid.NewGuid(),
                "Tomaten waschen und schneiden. Salat waschen und in einzelne Blätter teilen."),
            new(Guid.NewGuid(),
                "Eier hart kochen und anschließend in Scheiben schneiden.",
                600),
            new(Guid.NewGuid(),
                "Beide Scheiben Toast einseitig mit Mayonaise ca. 2mm beschmieren. Den Rest Mayonaise benötigen wir noch später. Über den Toast ein bischen Salz,Pfeffer und bei Bedarf auch Grillgewürz streuen. Das Grillgewürz verleiht dem Ganzem einen \"neuen\" Geschmack."),
        };

        return new RecipeDetailResponse(recipe.Id,
            username,
            recipe.Name,
            recipe.NormalizedName,
            recipe.Description,
            recipe.Servings,
            ingredients.ToArray(),
            steps.ToArray(),
            recipe.DurationWorkingMinutes,
            recipe.DurationCookingMinutes,
            recipe.DurationRestingMinutes,
            recipe.CaloriesKcal,
            recipe.Comments,
            recipe.Source);
    }
}
