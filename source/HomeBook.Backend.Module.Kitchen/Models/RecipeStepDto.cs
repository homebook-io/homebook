namespace HomeBook.Backend.Module.Kitchen.Models;

public record RecipeStepDto(
    Guid Id,
    string Description,
    int? TimerDurationInSeconds);
