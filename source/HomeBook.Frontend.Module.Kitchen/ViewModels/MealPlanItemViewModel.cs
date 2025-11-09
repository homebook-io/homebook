namespace HomeBook.Frontend.Module.Kitchen.ViewModels;

public class MealPlanItemViewModel
{
    public Guid Id { get; set; }
    public string ColorName { get; set; }
    public DateOnly Date { get; set; }
    public MealItemViewModel? Breakfast { get; set; }
    public MealItemViewModel? Lunch { get; set; }
    public MealItemViewModel? Dinner { get; set; }
}
