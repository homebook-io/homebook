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
    public string Ingredients { get; set; }
    public string Image { get; set; }

    public bool HasAnnotations => Duration.HasValue || Servings.HasValue;
}
