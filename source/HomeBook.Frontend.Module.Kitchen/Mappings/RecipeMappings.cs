using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeViewModel ToViewModel(this RecipeDto recipe)
    {
        TimeSpan? duration = recipe.DurationInMinutes.HasValue
            ? TimeSpan.FromMinutes(recipe.DurationInMinutes.Value)
            : null;

        return new RecipeViewModel
        {
            Id = recipe.Id,
            Username = recipe.Username,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            CaloriesKcal = recipe.CaloriesKcal,
            Duration = duration,
            Ingredients = recipe.Ingredients
        };
    }

    public static RecipeDto ToDto(this HomeBook.Client.Models.RecipeResponse r) =>
        new(
            r.Id!.Value,
            r.Username!,
            r.Name!,
            r.Description!,
            r.Servings,
            r.CaloriesKcal,
            r.DurationMinutes,
            "");
}
