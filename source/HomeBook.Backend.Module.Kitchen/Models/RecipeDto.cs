namespace HomeBook.Backend.Module.Kitchen.Models;

public record RecipeDto(
    Guid Id,
    Guid? UserId,
    string Name,
    string NormalizedName,
    string? Description,
    int? DurationMinutes,
    int? CaloriesKcal,
    int? Servings);
