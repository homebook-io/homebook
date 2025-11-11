using System.Diagnostics;

namespace HomeBook.Backend.DTOs.Responses.Kitchen;

[DebuggerDisplay("{Name}")]
public record RecipeResponse(
    Guid Id,
    string Name,
    string NormalizedName);
