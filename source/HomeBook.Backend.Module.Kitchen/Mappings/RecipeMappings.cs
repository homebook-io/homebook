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
            recipe.Source,
            recipe.Recipe2RecipeIngredient.Select(i => i.ToDto()).ToArray(),
            recipe.Steps.Select(s => s.ToDto()).ToArray());
    }

    public static RecipeIngredientDto ToDto(this Data.Entities.Recipe2RecipeIngredient r2ri)
    {
        return new RecipeIngredientDto(r2ri.IngredientId,
            r2ri.RecipeIngredient.Name,
            r2ri.RecipeIngredient.NormalizedName,
            r2ri.Quantity,
            r2ri.Unit);
    }

    public static RecipeStepDto ToDto(this Data.Entities.RecipeStep rs)
    {
        return new RecipeStepDto(
            rs.Id,
            rs.Description,
            rs.TimerDurationInSeconds);
    }

    public static async Task<RecipeResponse> ToResponseAsync(this RecipeDto r,
        Func<Guid, Task<UserInfo?>> getUserInfoAsync)
    {
        string? username = null;
        if (r.UserId.HasValue)
        {
            UserInfo? userInfo = await getUserInfoAsync(r.UserId.Value);
            username = userInfo?.Username;
        }

        return new RecipeResponse(r.Id,
            username,
            r.Name,
            r.NormalizedName,
            r.Description,
            r.DurationWorkingMinutes,
            r.CaloriesKcal,
            r.Servings);
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

        // var ingredients = new List<IngredientResponse>
        // {
        //     new(Guid.NewGuid(),
        //         1,
        //         "Scheibe",
        //         "Schinken",
        //         "gekochter, oder anderer Belag, z.B. Putenbrust oder Salami"),
        //     new(Guid.NewGuid(), 1, null, "Ei", "gekocht"),
        //     new(Guid.NewGuid(), 1, "Scheibe", "Käse", "am besten Emmentaler , Ihr könnt aber auch anderen nehmen"),
        //     new(Guid.NewGuid(), 1, null, "Salatblatt", null),
        //     new(Guid.NewGuid(), 0.5, null, "Tomate", null),
        //     new(Guid.NewGuid(), null, null, "Salz und Pfeffer, Grillgewürz", null),
        //     new(Guid.NewGuid(), 3, "EL", "Mayonaise", null),
        //     new(Guid.NewGuid(), 2, "Scheiben", "Sandwich Toast", null),
        //     new(Guid.NewGuid(), 4, "x", "Zahnstocher", "o.ä. zum Fixieren")
        // };
        //
        // var steps = new List<StepResponse>
        // {
        //     new(Guid.NewGuid(),
        //         "Tomaten waschen und schneiden. Salat waschen und in einzelne Blätter teilen."),
        //     new(Guid.NewGuid(),
        //         "Eier hart kochen und anschließend in Scheiben schneiden.",
        //         600),
        //     new(Guid.NewGuid(),
        //         "Beide Scheiben Toast einseitig mit Mayonaise ca. 2mm beschmieren. Den Rest Mayonaise benötigen wir noch später. Über den Toast ein bischen Salz,Pfeffer und bei Bedarf auch Grillgewürz streuen. Das Grillgewürz verleiht dem Ganzem einen \"neuen\" Geschmack."),
        // };

        return new RecipeDetailResponse(recipe.Id,
            username,
            recipe.Name,
            recipe.NormalizedName,
            recipe.Description,
            recipe.Servings,
            recipe.Ingredients.Select(x => x.ToResponse()).ToArray(),
            recipe.Steps.Select(x => x.ToResponse()).ToArray(),
            recipe.DurationWorkingMinutes,
            recipe.DurationCookingMinutes,
            recipe.DurationRestingMinutes,
            recipe.CaloriesKcal,
            recipe.Comments,
            recipe.Source);
    }

    public static RecipeIngredientResponse ToResponse(this RecipeIngredientDto ri)
    {
        return new RecipeIngredientResponse(ri.Id,
            ri.Name,
            ri.NormalizedName,
            ri.Quantity,
            ri.Unit);
    }

    public static RecipeStepResponse ToResponse(this RecipeStepDto rs)
    {
        return new RecipeStepResponse(
            rs.Id,
            rs.Description,
            rs.TimerDurationInSeconds);
    }
}
