using System.ComponentModel.DataAnnotations;
using HomeBook.Frontend.Abstractions.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings.Instance;

public partial class Overview : ComponentBase
{
    private MudForm _form = new();
    private bool _isValid;
    private bool _isLoading;
    private InstanceConfigurationModel _configurationModel = new();

    /// <summary>
    /// Instance configuration model for the form
    /// </summary>
    public class InstanceConfigurationModel
    {
        [Required(ErrorMessage = "Instance name is required")]
        [StringLength(255, ErrorMessage = "Instance name cannot exceed 255 characters")]
        [MinLength(1, ErrorMessage = "Instance name must have at least 1 character")]
        public string InstanceName { get; set; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentInstanceNameAsync();
    }

    /// <summary>
    /// Load the current instance name from the server
    /// </summary>
    private async Task LoadCurrentInstanceNameAsync()
    {
        try
        {
            _isLoading = true;

            CancellationToken cancellationToken = CancellationToken.None;
            string instanceName = await InstanceManagementProvider.GetInstanceNameAsync(cancellationToken);

            _configurationModel.InstanceName = instanceName;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading instance configuration: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// Update the instance name on the server
    /// </summary>
    private async Task UpdateInstanceNameAsync()
    {
        if (!_isValid)
            return;

        try
        {
            _isLoading = true;

            // Validate form before proceeding
            await _form.Validate();
            if (!_form.IsValid)
                return;

            string instanceName = _configurationModel.InstanceName.Trim();
            CancellationToken cancellationToken = CancellationToken.None;

            await InstanceManagementProvider.UpdateInstanceNameAsync(instanceName, cancellationToken);

            await JsLocalStorageProvider.SetItemAsync(JsLocalStorageKeys.HomeBookInstanceName,
                instanceName,
                cancellationToken);

            Snackbar.Add("Instance name updated successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating instance name: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }
}
