using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.MealPlan;

public partial class PlanOverview : ComponentBase
{
    private List<MealPlanItemViewModel> _mealPlanItems = [];
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(6);
    private short _calendarWeek = (short)System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        // Simulate data fetching
        _mealPlanItems =
        [
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                ColorName = "cerulean",
                Breakfast = new MealItemViewModel()
                {
                    Name = "Omelette mit Speck",
                    Ingredients = "Eier, Speck, Milch, Gewürze",
                    Duration = TimeSpan.FromMinutes(15),
                    CaloriesKcal = 350
                },
                Lunch = new MealItemViewModel()
                {
                    Name = "Würstchen mit Kartoffelsalat",
                    Ingredients = "Würstchen, Kartoffeln, Mayonnaise, Zwiebeln, Gurken",
                    Duration = TimeSpan.FromMinutes(135),
                    CaloriesKcal = 800
                },
                Dinner = new MealItemViewModel()
                {
                    Name = "Bratkartoffeln mit Spiegelei",
                    Ingredients = "Kartoffeln, Eier, Zwiebeln, Gewürze",
                    Duration = TimeSpan.FromMinutes(105)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(1),
                ColorName = "fern",
                Lunch = new MealItemViewModel()
                {
                    Name = "Würstchen mit Kartoffelsalat",
                    Ingredients = "Würstchen, Kartoffeln, Mayonnaise, Zwiebeln, Gurken",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(2),
                ColorName = "amber",
                Dinner = new MealItemViewModel()
                {
                    Name = "Bratkartoffeln mit Spiegelei",
                    Ingredients = "Kartoffeln, Eier, Zwiebeln, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(3),
                ColorName = "azure",
                Breakfast = new MealItemViewModel()
                {
                    Name = "Fischstäbchen mit Kartoffelpüree",
                    Ingredients = "Fischstäbchen, Kartoffeln, Butter, Milch",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(4),
                ColorName = "chartreuse",
                Breakfast = new MealItemViewModel()
                {
                    Name = "Eintopf mit Würstchen",
                    Ingredients = "Würstchen, Kartoffeln, Gemüse, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(5),
                ColorName = "jade",
                Breakfast = new MealItemViewModel()
                {
                    Name = "Hähnchenschenkel mit Reis",
                    Ingredients = "Hähnchenschenkel, Reis, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(6),
                ColorName = "plum",
                Breakfast = new MealItemViewModel()
                {
                    Name = "Roastbeef mit Gemüse",
                    Ingredients = "Kartoffeln, Gemüse, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            }
        ];

        StateHasChanged();
    }
}
