using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace HomeBook.Frontend.Pages.Settings.UserViews;

public partial class UserAdd : ComponentBase
{
    private readonly UserAddModel _userModel = new();
    private bool _saving = false;

    // Password visibility toggles
    private bool _showPassword = false;
    private bool _showConfirmPassword = false;
    private InputType _passwordInputType = InputType.Password;
    private InputType _confirmPasswordInputType = InputType.Password;
    private string _passwordIcon = Icons.Material.Filled.VisibilityOff;
    private string _confirmPasswordIcon = Icons.Material.Filled.VisibilityOff;

    private readonly List<BreadcrumbItem> _breadcrumbs = new()
    {
        new BreadcrumbItem("Settings", href: "/Settings", icon: Icons.Material.Filled.Settings),
        new BreadcrumbItem("Users", href: "/Settings/Users", icon: Icons.Material.Filled.People),
        new BreadcrumbItem("Add User", href: null, disabled: true)
    };

    private void TogglePasswordVisibility()
    {
        if (_showPassword)
        {
            _showPassword = false;
            _passwordInputType = InputType.Password;
            _passwordIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            _showPassword = true;
            _passwordInputType = InputType.Text;
            _passwordIcon = Icons.Material.Filled.Visibility;
        }
    }

    private void ToggleConfirmPasswordVisibility()
    {
        if (_showConfirmPassword)
        {
            _showConfirmPassword = false;
            _confirmPasswordInputType = InputType.Password;
            _confirmPasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            _showConfirmPassword = true;
            _confirmPasswordInputType = InputType.Text;
            _confirmPasswordIcon = Icons.Material.Filled.Visibility;
        }
    }

    private async Task HandleValidSubmitAsync()
    {
        // Manual validation for password confirmation
        if (_userModel.Password != _userModel.ConfirmPassword)
        {
            Snackbar.Add("Passwords do not match", Severity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(_userModel.Username))
        {
            Snackbar.Add("Username is required", Severity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(_userModel.Password))
        {
            Snackbar.Add("Password is required", Severity.Error);
            return;
        }

        if (_userModel.Username.Length < 5 || _userModel.Username.Length > 20)
        {
            Snackbar.Add("Username must be between 5 and 20 characters", Severity.Error);
            return;
        }

        if (_userModel.Password.Length < 8)
        {
            Snackbar.Add("Password must be at least 8 characters long", Severity.Error);
            return;
        }

        try
        {
            _saving = true;

            Guid? createdUserId = await UserManagementProvider.CreateUserAsync(_userModel.Username,
                _userModel.Password,
                _userModel.IsAdmin,
                CancellationToken.None);

            if (createdUserId is null)
            {
                Snackbar.Add($"User '{_userModel.Username}' could not be created", Severity.Error);
                return;
            }

            NavigationManager.NavigateTo($"/Settings/Users/{createdUserId}");
            Snackbar.Add($"User '{_userModel.Username}' created successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error creating user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/Settings/Users");
    }

    private class UserAddModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;
    }
}
