using System.Diagnostics;

namespace HomeBook.Backend.Module.Kitchen.Responses;

[DebuggerDisplay("{Quantity} {Unit} {Name} -  {AdditionalText}")]
public record IngredientResponse(
    Guid Id,
    double? Quantity,
    string? Unit,
    string Name,
    string? AdditionalText);
