using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Pages.Recipes.Components;

public partial class UiRecipeDescriptionServings : ComponentBase
{
    [Parameter]
    public string Description { get; set; } = string.Empty;

    protected int _numberOfServings;

    [Parameter]
    public int NumberOfServings
    {
        get => _numberOfServings;
        set => _numberOfServings = value;
    }

    [Parameter]
    public EventCallback<int> NumberOfServingsChanged { get; set; }

    private async Task OnNumberOfServingsChanged(int value)
    {
        await NumberOfServingsChanged.InvokeAsync(value);
        StateHasChanged();
    }
}
