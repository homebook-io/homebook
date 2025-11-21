namespace HomeBook.Frontend.Module.Kitchen.ViewModels;

public class MealPlanItemViewModel
{
    public Guid Id { get; set; }
    public string ColorName { get; set; }
    public DateOnly Date { get; set; }
    public RecipeViewModel? Breakfast { get; set; }
    public RecipeViewModel? Lunch { get; set; }
    public RecipeViewModel? Dinner { get; set; }
}
