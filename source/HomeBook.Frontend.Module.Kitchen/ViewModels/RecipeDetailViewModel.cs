namespace HomeBook.Frontend.Module.Kitchen.ViewModels;

public class RecipeDetailViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Servings { get; set; }
    public int? CaloriesKcal { get; set; }
    public TimeSpan? Duration { get; set; }
    public TimeSpan? DurationWorkingMinutes { get; set; }
    public TimeSpan? DurationCookingMinutes { get; set; }
    public TimeSpan? DurationRestingMinutes { get; set; }
    public IEnumerable<IngredientViewModel> Ingredients { get; set; }
    public IEnumerable<StepViewModel> Steps { get; set; }
    public string Image { get; set; }
    public string Source { get; set; }
    public string Comment { get; set; }

    public int NumberOfServings { get; set; }

    public bool HasAnnotations => Duration.HasValue || Servings.HasValue;

    public RecipeDetailViewModel()
    {
        NumberOfServings = Servings ?? 1;
    }
}
