using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes.Components;

public partial class UiRecipeStepsList : ComponentBase
{
    [Parameter]
    public IEnumerable<StepViewModel> Steps { get; set; } = new List<StepViewModel>();
}
