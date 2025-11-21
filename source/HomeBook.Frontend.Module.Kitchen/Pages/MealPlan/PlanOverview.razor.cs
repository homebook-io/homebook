using HomeBook.Frontend.Module.Kitchen.Dialogs;
using HomeBook.Frontend.Module.Kitchen.Enums;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Module.Kitchen.Pages.MealPlan;

public partial class PlanOverview : ComponentBase
{
    private List<MealPlanItemViewModel> _mealPlanItems = [];
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(6);

    private short _calendarWeek =
        (short)System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today,
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);

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
                Date = DateOnly.FromDateTime(DateTime.Today),
                ColorName = "cerulean",
                Breakfast = new RecipeViewModel()
                {
                    Name = "Omelette mit Speck",
                    Ingredients = "Eier, Speck, Milch, Gewürze",
                    Duration = TimeSpan.FromMinutes(15),
                    CaloriesKcal = 350
                },
                Lunch = new RecipeViewModel()
                {
                    Name = "Würstchen mit Kartoffelsalat",
                    Ingredients = "Würstchen, Kartoffeln, Mayonnaise, Zwiebeln, Gurken",
                    Duration = TimeSpan.FromMinutes(135),
                    CaloriesKcal = 800
                },
                Dinner = new RecipeViewModel()
                {
                    Name = "Bratkartoffeln mit Spiegelei",
                    Ingredients = "Kartoffeln, Eier, Zwiebeln, Gewürze",
                    Duration = TimeSpan.FromMinutes(105)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                ColorName = "fern",
                Lunch = new RecipeViewModel()
                {
                    Name = "Würstchen mit Kartoffelsalat",
                    Ingredients = "Würstchen, Kartoffeln, Mayonnaise, Zwiebeln, Gurken",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                ColorName = "amber",
                Dinner = new RecipeViewModel()
                {
                    Name = "Bratkartoffeln mit Spiegelei",
                    Ingredients = "Kartoffeln, Eier, Zwiebeln, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                ColorName = "azure",
                Breakfast = new RecipeViewModel()
                {
                    Name = "Fischstäbchen mit Kartoffelpüree",
                    Ingredients = "Fischstäbchen, Kartoffeln, Butter, Milch",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
                ColorName = "chartreuse",
                Breakfast = new RecipeViewModel()
                {
                    Name = "Eintopf mit Würstchen",
                    Ingredients = "Würstchen, Kartoffeln, Gemüse, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                ColorName = "jade",
                Breakfast = new RecipeViewModel()
                {
                    Name = "Hähnchenschenkel mit Reis",
                    Ingredients = "Hähnchenschenkel, Reis, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                ColorName = "plum",
                Breakfast = new RecipeViewModel()
                {
                    Name = "Roastbeef mit Gemüse",
                    Ingredients = "Kartoffeln, Gemüse, Gewürze",
                    Duration = TimeSpan.FromMinutes(30)
                }
            }
        ];

        StateHasChanged();
    }

    private async Task OnMealAdd(MealType mealType, DateOnly date)
    {
        IDialogReference dialogReference = await DialogService.ShowAsync<MealSelectDialog>(
            "+Gericht auswählen",
            new DialogOptions()
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            });

        DialogResult? dialogResult = await dialogReference.Result;
        if (dialogResult is null)
            return;

        RecipeViewModel meal = (dialogResult.Data as RecipeViewModel)!;

        MealPlanItemViewModel? mealPlanItem = _mealPlanItems.FirstOrDefault(item => item.Date == date);
        switch (mealType)
        {
            case MealType.Breakfast:
                mealPlanItem!.Breakfast = meal;
                break;
            case MealType.Lunch:
                mealPlanItem!.Lunch = meal;
                break;
            case MealType.Dinner:
                mealPlanItem!.Dinner = meal;
                break;
        }

        StateHasChanged();
    }

    private async Task OnMealDelete(MealType mealType, DateOnly date)
    {
        MealPlanItemViewModel? mealPlanItem = _mealPlanItems.FirstOrDefault(item => item.Date == date);
        switch (mealType)
        {
            case MealType.Breakfast:
                mealPlanItem!.Breakfast = null;
                break;
            case MealType.Lunch:
                mealPlanItem!.Lunch = null;
                break;
            case MealType.Dinner:
                mealPlanItem!.Dinner = null;
                break;
        }

        StateHasChanged();
    }
}
