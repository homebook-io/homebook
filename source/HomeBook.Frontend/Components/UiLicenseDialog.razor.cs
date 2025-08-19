using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Components;

public partial class UiLicenseDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();

    private async Task AcceptLicensesAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        // await StepSuccessAsync(cancellationToken);
        // await InvokeAsync(StateHasChanged);
    }
}
