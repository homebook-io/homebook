using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace HomeBook.Frontend.Components;

public partial class UiRedirectToLogin : ComponentBase
{
    protected override void OnInitialized()
    {
        string currentUrl = NavigationManager.Uri;
        string returnUrl = Uri.EscapeDataString(currentUrl);
        NavigationManager.NavigateTo($"/Login?returnUrl={returnUrl}");
    }
}
