using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class MealMappings
{
    public static MealItemViewModel ToViewModel(this Models.Meal meal)
    {
        return new MealItemViewModel
        {
            Name = meal.Name,
            Ingredients = meal.Ingredients,
            Duration = meal.Duration,
            CaloriesKcal = meal.CaloriesKcal
        };
    }
}
