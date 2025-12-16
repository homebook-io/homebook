using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes.Components;

public partial class UiRecipeIngredientsList : ComponentBase
{
    [Parameter]
    public IEnumerable<IngredientViewModel> Ingredients { get; set; } = new List<IngredientViewModel>();
}
