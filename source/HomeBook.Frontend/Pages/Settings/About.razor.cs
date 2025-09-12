using HomeBook.Client.Models;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Pages.Settings;

public partial class About : ComponentBase
{
    private string _uiVersion = "1.0.0";
    private string _uiDotnetVersion = "1.0.0";
    private string _backendVersion = "1.0.0";
    private string _backendDotnetVersion = "1.0.0";
    private string _databaseProvider = "";
    private string _deploymentType = "";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        CancellationToken cancellationToken = CancellationToken.None;

        await LoadUIInfoAsync(cancellationToken);
        await LoadBackendInfoAsync(cancellationToken);
    }

    private async Task LoadUIInfoAsync(CancellationToken cancellationToken)
    {
        _uiVersion = Configuration["AppVersion"] ?? "1.0.0";
        _uiDotnetVersion = System.Environment.Version.ToString();
    }

    private async Task LoadBackendInfoAsync(CancellationToken cancellationToken)
    {
        GetSystemInfoResponse? response = await BackendClient.System.GetAsync(x =>
            {
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
}
