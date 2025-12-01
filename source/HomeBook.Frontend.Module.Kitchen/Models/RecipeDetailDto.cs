namespace HomeBook.Frontend.Module.Kitchen.Models;

public record RecipeDetailDto(
    Guid Id,
    string Username,
    string Name,
    string Description,
    int? Servings,
    int? CaloriesKcal,
    int? DurationInMinutes,
    string Ingredients);
