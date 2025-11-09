namespace HomeBook.Frontend.Module.Kitchen.Models;

public record Meal(
    string Name,
    string Ingredients,
    TimeSpan? Duration,
    int? CaloriesKcal);
