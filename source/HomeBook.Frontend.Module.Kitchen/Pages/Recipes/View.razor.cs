using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes;

public partial class View : ComponentBase
{
    [Parameter]
    public Guid RecipeId { get; set; }

    private bool _isLoading = false;
}
