using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class MealMappings
{
    public static MealItemViewModel ToViewModel(this RecipeDto recipe)
    {
        return new MealItemViewModel
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
