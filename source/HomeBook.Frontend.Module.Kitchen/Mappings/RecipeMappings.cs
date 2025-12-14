using HomeBook.Client.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeDetailViewModel ToViewModel(this RecipeDetailDto recipe)
    {
        TimeSpan? duration = recipe.DurationInMinutes.HasValue
            ? TimeSpan.FromMinutes(recipe.DurationInMinutes.Value)
            : null;

        return new RecipeDetailViewModel
        {
            Id = recipe.Id,
            Username = recipe.Username,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            CaloriesKcal = recipe.CaloriesKcal,
            Duration = duration,
            // Ingredients = recipe.Ingredients,
            Image = TestImageMappings.PlaceholderImage
        };
    }

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
            Ingredients = recipe.Ingredients,
            Image = TestImageMappings.PlaceholderImage
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
            r.DurationCookingMinutes,
            "");

    public static RecipeDetailDto ToDto(this HomeBook.Client.Models.RecipeDetailResponse r) =>
        new(
            r.Id!.Value,
            r.Username!,
            r.Name!,
            r.Description!,
            r.Servings,
            r.CaloriesKcal,
            r.DurationCookingMinutes,
            "");

    public static CreateRecipeStepRequest ToRequest(this RecipeStepDto dto) =>
        new()
        {
            Description = dto.Description,
            Position = dto.Position,
            TimerDurationInSeconds = dto.TimerDurationInSeconds
        };

    public static CreateRecipeIngredientRequest ToRequest(this RecipeIngredientDto dto) =>
        new()
        {
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
        };
}
