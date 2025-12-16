using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes.Components;

public partial class UiRecipeDescriptionServings : ComponentBase
{
    [Parameter]
    public string Description { get; set; } = string.Empty;

    [Parameter]
    public int Servings { get; set; } = 1;
}
