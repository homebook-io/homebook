using HomeBook.Frontend.Module.Kitchen.Mappings;
using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes;

public partial class Overview : ComponentBase
{
    private List<RecipeViewModel> _recipes = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        // TODO: load data
        await LoadRecipesAsync();
    }

    private async Task LoadRecipesAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        try
        {
            IEnumerable<RecipeDto> recipes = await RecipeService.GetRecipesAsync(string.Empty,
                cancellationToken);
            _recipes.Clear();
            foreach (RecipeDto recipe in recipes)
            {
                _recipes.Add(recipe.ToViewModel());
            }
        }
        catch (Exception err)
        {
            int i = 0;
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task CreateRecipeAsync()
    {
        string recipeName = "Cheeseburger";
        string description =
            "Ein leckerer Cheeseburger mit saftigem Rindfleisch, geschmolzenem Käse, frischem Salat, Tomaten und Zwiebeln, serviert in einem weichen Brötchen.";
        int durationInMinutes = 45;
        int caloriesKcal = 3250;
        int servings = 2;

        await RecipeService.CreateRecipeAsync(recipeName,
            description,
            durationInMinutes,
            caloriesKcal,
            servings);
    }
}
