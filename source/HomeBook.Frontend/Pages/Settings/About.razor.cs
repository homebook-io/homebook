using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Abstractions.Models.System;
using HomeBook.Frontend.Components;
using HomeBook.Frontend.Core.Models.Setup;
using HomeBook.Frontend.Mappings;
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

        LoadUiInfoAsync();
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

    private void LoadUiInfoAsync()
    {
        _uiVersion = Configuration["AppVersion"] ?? "1.0.0";
        _uiDotnetVersion = Environment.Version.ToString();
    }

    private async Task LoadBackendInfoAsync(CancellationToken cancellationToken)
    {
        if (!await AuthenticationService.IsCurrentUserAdminAsync(cancellationToken))
            return;

        try
        {
            SystemInfo? systemInfo = await SystemManagementProvider.GetSystemInfoAsync(cancellationToken);
            if (systemInfo is not null)
            {
                _backendVersion = systemInfo.AppVersion.ToString();
                _backendDotnetVersion = systemInfo.DotNetVersion;

                _databaseProvider = systemInfo.DatabaseProvider;
                _deploymentType = systemInfo.DeploymentType;
            }
        }
        catch (UnauthorizedAccessException)
        {
            // user is no admin
            Snackbar.Add("You are not authorized to view system information.", Severity.Warning);
        }
        catch (Exception err)
        {
            // display error
            Snackbar.Add($"Loading system information failed: {err.Message}", Severity.Error);
        }
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
        if (licenseDialogResult is null
            || licenseDialogResult.Canceled)
            return;
    }
}
