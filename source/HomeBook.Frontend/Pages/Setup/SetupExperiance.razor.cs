using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Pages.Setup;

public partial class SetupExperiance : ComponentBase
{
    private string _appVersion = string.Empty;
    private string _appServer = string.Empty;
    private string _setupTiledBackgroundClass = "";
    private string _setupContainerClass = "";
    private string _setupBackgroundClass = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        _appVersion = Configuration["Version"] ?? "Unknown";
        _appServer = Configuration["Backend:Host"] ?? "Unknown";
        StateHasChanged();
    }

    private async Task FinishSetupAnimationAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        // hide content
        _setupContainerClass = "is-finished";
        StateHasChanged();

        // move tiles from center to outside
        _setupTiledBackgroundClass = "is-finished";
        StateHasChanged();

        await Task.Delay(2000, cancellationToken);

        // fade out background
        _setupBackgroundClass = "is-finished";
        StateHasChanged();

        await Task.Delay(1000, cancellationToken);

        // TODO: navigate to homebook start page
    }
}
