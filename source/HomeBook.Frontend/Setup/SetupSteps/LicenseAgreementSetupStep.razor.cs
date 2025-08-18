using HomeBook.Frontend.Abstractions.Contracts;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Setup.SetupSteps;

public partial class LicenseAgreementSetupStep : ComponentBase, ISetupStep
{
    private string _licenseHtmlContent = string.Empty;

    public string Key { get; } = nameof(LicenseAgreementSetupStep);
    public bool HasError { get; set; }
    public bool IsSuccessful { get; set; }
    public Task HandleStepAsync() => throw new NotImplementedException();

    public Task<bool> IsStepDoneAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        // TODO: load licenses
    }
}
