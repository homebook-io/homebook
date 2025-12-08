using System.Diagnostics;

namespace HomeBook.Backend.Module.Kitchen.Responses;

[DebuggerDisplay("{Description}")]
public record StepResponse(
    Guid Id,
    string Description,
    int? TimerDurationInSeconds = null);
