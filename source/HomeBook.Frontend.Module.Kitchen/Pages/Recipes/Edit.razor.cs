using HomeBook.Frontend.Module.Kitchen.Mappings;
using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes;

public partial class Edit : ComponentBase
{
    [Parameter]
    public Guid RecipeId { get; set; }

    private bool _isLoading = false;
    private RecipeDetailViewModel? _recipe = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        await LoadRecipeAsync();
    }

    private async Task LoadRecipeAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        _isLoading = true;
        StateHasChanged();

        try
        {
            RecipeDetailDto? recipeDto = await RecipeService.GetRecipeByIdAsync(RecipeId,
                cancellationToken);
            if (recipeDto is null)
            {
                // recipe not found
                Snackbar.Add("+Recipe could not be found.", Severity.Error);
                NavigationManager.NavigateTo("/Kitchen/Recipes");
            }

            _recipe = recipeDto.ToViewModel();
        }
        catch (Exception err)
        {
            int i = 0;
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task DeleteRecipe()
    {
        try
        {
            await RecipeService.DeleteRecipeAsync(RecipeId);
            Snackbar.Add("+Recipe deleted successfully.", Severity.Success);

            NavigationManager.NavigateTo("/Kitchen/Recipes");
        }
        catch (Exception err)
        {
            Snackbar.Add("+Recipe could not be deleted. " + err.Message, Severity.Error);
        }
    }

    private async Task SaveRecipeAsync()
    {
        await RecipeService.CreateOrUpdateRecipeAsync(RecipeId,
            _recipe!.Name,
            _recipe.Description,
            _recipe.Servings,
            _recipe.Steps?.Select((s, i) => s.ToDto(i)).ToArray(),
            _recipe.Ingredients?.Select(i => i.ToDto()).ToArray(),
            Convert.ToInt32(_recipe.DurationWorkingMinutes?.TotalSeconds),
            Convert.ToInt32(_recipe.DurationCookingMinutes?.TotalSeconds),
            Convert.ToInt32(_recipe.DurationRestingMinutes?.TotalSeconds),
            _recipe.CaloriesKcal,
            _recipe.Comments,
            _recipe.Source);
    }

    private void AbortEditingRecipe()
    {
        NavigationManager.NavigateTo("/Kitchen/Recipes");
    }
}
