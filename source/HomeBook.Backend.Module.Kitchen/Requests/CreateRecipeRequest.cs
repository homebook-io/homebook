namespace HomeBook.Backend.Module.Kitchen.Requests;

public record CreateRecipeRequest(
    string Name,
    string? Description,
    TimeSpan? Duration,
    int? CaloriesKcal,
    int? Servings);
