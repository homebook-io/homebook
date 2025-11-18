using System.Diagnostics;

namespace HomeBook.Backend.Module.Kitchen.Responses;

[DebuggerDisplay("{Name}")]
public record RecipeResponse(
    Guid Id,
    string Name,
    string NormalizedName);
