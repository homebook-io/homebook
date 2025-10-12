using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Components;

public partial class UiPageTitle : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
