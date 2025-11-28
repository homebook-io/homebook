using HomeBook.Frontend.Module.Kitchen.Enums;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using HomeBook.Frontend.UI.Utilities;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Components;

public partial class UiMealDay : ComponentBase
{
    [Parameter]
    public MealPlanItemViewModel MealPlanItem { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ArgumentNullException.ThrowIfNull(MealPlanItem, nameof(MealPlanItem));
    }

    protected string HeaderCssClass =>
        new HtmlArgumentBuilder("ui-meal-card")
            .AddClass("border-0")
            .AddClass($"ui-color-bg-gradient-{MealPlanItem!.ColorName}",
                !string.IsNullOrEmpty(MealPlanItem.ColorName))
            .Build();

    private async Task OnMealAdd(MealType mealType, DateOnly date)
    {
        // IDialogReference dialogReference = await DialogService.ShowAsync<MealSelectDialog>(
        //     "+Gericht auswählen",
        //     new DialogOptions()
        //     {
        //         MaxWidth = MaxWidth.Small,
        //         FullWidth = true,
        //         CloseOnEscapeKey = true,
        //         CloseButton = true
        //     });
        //
        // DialogResult? dialogResult = await dialogReference.Result;
        // if (dialogResult is null)
        //     return;
        //
        // RecipeViewModel meal = (dialogResult.Data as RecipeViewModel)!;

        // MealPlanItemViewModel? mealPlanItem = _mealPlanItems.FirstOrDefault(item => item.Date == date);
        // switch (mealType)
        // {
        //     case MealType.Breakfast:
        //         mealPlanItem!.Breakfast = meal;
        //         break;
        //     case MealType.Lunch:
        //         mealPlanItem!.Lunch = meal;
        //         break;
        //     case MealType.Dinner:
        //         mealPlanItem!.Dinner = meal;
        //         break;
        // }
        //
        // StateHasChanged();
    }

    private async Task OnMealDelete(MealType mealType, DateOnly date)
    {
        // MealPlanItemViewModel? mealPlanItem = _mealPlanItems.FirstOrDefault(item => item.Date == date);
        // switch (mealType)
        // {
        //     case MealType.Breakfast:
        //         mealPlanItem!.Breakfast = null;
        //         break;
        //     case MealType.Lunch:
        //         mealPlanItem!.Lunch = null;
        //         break;
        //     case MealType.Dinner:
        //         mealPlanItem!.Dinner = null;
        //         break;
        // }
        //
        // StateHasChanged();
    }
}
