using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeViewModel ToViewModel(this RecipeDto recipe)
    {
        return new RecipeViewModel
        {
            Name = recipe.Name,
            Ingredients = recipe.Ingredients,
            Duration = recipe.Duration,
            CaloriesKcal = recipe.CaloriesKcal
        };
    }

    public static RecipeDto ToDto(this HomeBook.Client.Models.RecipeResponse r) =>
        new(
            r.Id!.Value,
            r.Name!,
            "",
            null,
            null);
}
