using HomeBook.Frontend.Module.Kitchen.ViewModels;
using HomeBook.Frontend.UI.Components;
using HomeBook.Frontend.UI.Utilities;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Kitchen.Components;

public partial class UiMealCard : ComponentBase
{
    protected string Style =>
        new HtmlArgumentBuilder("")

            /* UiIconStyle == Filled */
            .AddClass($"background-color: color-mix(in srgb, {MealTypeColor}, transparent 90%);")

            .Build();

    [Parameter]
    public string MealTypeName { get; set; } = string.Empty;

    [Parameter]
    public string MealTypeIcon { get; set; } = string.Empty;

    [Parameter]
    public string MealTypeColor { get; set; } = string.Empty;

    [Parameter]
    public MealItemViewModel? Meal { get; set; } = null;
}
