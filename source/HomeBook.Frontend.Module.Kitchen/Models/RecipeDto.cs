namespace HomeBook.Frontend.Module.Kitchen.Models;

public record RecipeDto(
    Guid Id,
    string Name,
    string Ingredients,
    TimeSpan? Duration,
    int? CaloriesKcal);
