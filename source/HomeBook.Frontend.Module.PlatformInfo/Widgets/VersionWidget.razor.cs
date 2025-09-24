using HomeBook.Frontend.Modules.Abstractions;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.PlatformInfo.Widgets;

public partial class VersionWidget : ComponentBase, IWidget
{
    private string _version = string.Empty;

    public static WidgetSize[] AvailableSizes { get; } =
    [
        WidgetSize.Size2x1
    ];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        _version = Configuration["AppVersion"] ?? "1.0.0";
        StateHasChanged();
    }
}
