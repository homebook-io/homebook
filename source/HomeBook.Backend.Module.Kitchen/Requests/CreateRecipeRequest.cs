namespace HomeBook.Backend.Module.Kitchen.Requests;

public record CreateRecipeRequest(
    string Name,
    string? Description,
    int? DurationInMinutes,
    int? CaloriesKcal,
    int? Servings);
