using System.Diagnostics;

namespace HomeBook.Backend.Module.Kitchen.Responses;

[DebuggerDisplay("{Name}")]
public record RecipeResponse(
    Guid Id,
    string? Username,
    string Name,
    string NormalizedName,
    string? Description,
    int? DurationMinutes,
    int? CaloriesKcal,
    int? Servings);
