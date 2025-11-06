using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.MealPlan;

public partial class PlanOverview : ComponentBase
{
    private List<MealPlanItemViewModel> _mealPlanItems = [];

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
                Name = "Spaghetti Bolognese",
                Date = DateTime.Today,
                ColorName = "cerulean"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Würstchen mit Kartoffelsalat",
                Date = DateTime.Today.AddDays(1),
                ColorName = "fern"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Bratkartoffeln mit Spiegelei",
                Date = DateTime.Today.AddDays(2),
                ColorName = "amber"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Fischstäbchen mit Kartoffelpüree",
                Date = DateTime.Today.AddDays(3),
                ColorName = "azure"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Eintopf mit Würstchen",
                Date = DateTime.Today.AddDays(4),
                ColorName = "chartreuse"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Hähnchenschenkel mit Reis",
                Date = DateTime.Today.AddDays(5),
                ColorName = "jade"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Roastbeef mit Gemüse",
                Date = DateTime.Today.AddDays(6),
                ColorName = "plum"
            }
        ];

        StateHasChanged();
    }
}
