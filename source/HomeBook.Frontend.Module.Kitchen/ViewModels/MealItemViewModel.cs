namespace HomeBook.Frontend.Module.Kitchen.ViewModels;

public class MealItemViewModel
{
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? CaloriesKcal { get; set; }
}
