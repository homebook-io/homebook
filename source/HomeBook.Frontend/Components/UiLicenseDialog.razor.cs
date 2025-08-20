using HomeBook.Frontend.Models.Setup;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Components;

public partial class UiLicenseDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public EventCallback OnCanceled { get; set; }

    [Parameter]
    public EventCallback OnAccepted { get; set; }

    [Parameter]
    public List<LicenseViewModel> Licenses { get; set; } = [];

    private async Task CancelAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        if (OnCanceled.HasDelegate)
            await OnCanceled.InvokeAsync(cancellationToken);

        MudDialog.Cancel();
    }

    private async Task AcceptLicensesAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        if (OnAccepted.HasDelegate)
            await OnAccepted.InvokeAsync(cancellationToken);

        MudDialog.Close(DialogResult.Ok(true));
    }
}
