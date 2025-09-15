using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Components;
using HomeBook.Frontend.Mappings;
using HomeBook.Frontend.Models.Setup;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings;

public partial class About : ComponentBase
{
    private string _uiVersion = "1.0.0";
    private string _uiDotnetVersion = "1.0.0";
    private string _backendVersion = "1.0.0";
    private string _backendDotnetVersion = "1.0.0";
    private string _databaseProvider = "";
    private string _deploymentType = "";
    private List<LicenseViewModel> _licenses = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        CancellationToken cancellationToken = CancellationToken.None;

        await LoadUiInfoAsync(cancellationToken);
        await LoadBackendInfoAsync(cancellationToken);
        await LoadLicensesAsync(cancellationToken);
    }

    private async Task LoadLicensesAsync(CancellationToken cancellationToken)
    {
        License[] licenses = await LicensesService.GetAllLicensesAsync(cancellationToken);
        _licenses = licenses.OrderBy(x => x.Name)
            .Select(x => x.ToViewModel())
            .ToList();
    }

    private async Task LoadUiInfoAsync(CancellationToken cancellationToken)
    {
        _uiVersion = Configuration["AppVersion"] ?? "1.0.0";
        _uiDotnetVersion = System.Environment.Version.ToString();
    }

    private async Task LoadBackendInfoAsync(CancellationToken cancellationToken)
    {
        string? token = await AuthenticationService.GetTokenAsync(cancellationToken);
        if (string.IsNullOrEmpty(token))
        {
            Snackbar.Add("Authentication token not found", Severity.Error);
            return;
        }

        GetSystemInfoResponse? response = await BackendClient.System.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);

        _backendVersion = response.AppVersion;
        _backendDotnetVersion = response.DotnetRuntimeVersion;

        _databaseProvider = response.DatabaseProvider.ToUpperInvariant() switch
        {
            "POSTGRESQL" => "PostgreSQL",
            "MYSQL" => "MySQL",
            "MARIADBl" => "MariaDB",
            _ => response.DatabaseProvider
        };
        _deploymentType = response.DeploymentType.ToUpperInvariant() switch
        {
            "DOCKER" => "Docker",
            _ => response.DeploymentType
        };
    }

    private async Task OpenLicensesDialogAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        DialogParameters<UiLicenseDialog> parameters = new()
        {
            {
                x => x.Licenses, _licenses
            },
            {
                x => x.ShowAcceptButton, false
            },
        };

        IDialogReference licenseDialog = await DialogService.ShowAsync<UiLicenseDialog>("HomeBook Licenses",
            parameters,
            new DialogOptions()
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
            });

        DialogResult? licenseDialogResult = await licenseDialog.Result;
        if (licenseDialogResult.Canceled)
            return;
    }
}
