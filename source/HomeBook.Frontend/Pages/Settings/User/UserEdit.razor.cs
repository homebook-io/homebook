using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using HomeBook.Frontend.Abstractions.Models.System;
using HomeBook.Frontend.Core.Models.Settings.Users;

namespace HomeBook.Frontend.Pages.Settings.User;

public partial class UserEdit : ComponentBase
{
    [Parameter]
    public Guid UserId { get; set; }

    private UserEditModel? _userModel;
    private UserData? _originalUser;
    private bool _loading = true;
    private bool _saving = false;
    private Guid? _currentUserId;

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
        new BreadcrumbItem("Edit User", href: null, disabled: true)
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUserAsync();
        await LoadUserAsync();
    }

    private async Task LoadCurrentUserAsync()
    {
        try
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                Claim? userIdClaim = authState.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    _currentUserId = userId;
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading current user: {ex.Message}", Severity.Error);
        }
    }

    private async Task LoadUserAsync()
    {
        try
        {
            _loading = true;

            // Get users and find the specific one
            _originalUser = await UserManagementProvider.GetUserByIdAsync(UserId, CancellationToken.None);

            if (_originalUser != null)
            {
                _userModel = new UserEditModel
                {
                    Username = _originalUser.UserName,
                    Password = string.Empty,
                    ConfirmPassword = string.Empty,
                    IsAdmin = _originalUser.IsAdmin
                };
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private bool IsCurrentUser()
    {
        return _currentUserId.HasValue && _currentUserId.Value == UserId;
    }

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
        if (_userModel == null
            || _originalUser == null)
            return;

        // Password validation only if password is being changed
        if (!string.IsNullOrWhiteSpace(_userModel.Password))
        {
            if (_userModel.Password != _userModel.ConfirmPassword)
            {
                Snackbar.Add("Passwords do not match", Severity.Error);
                return;
            }

            if (_userModel.Password.Length < 8)
            {
                Snackbar.Add("Password must be at least 8 characters long", Severity.Error);
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(_userModel.Username))
        {
            Snackbar.Add("Username is required", Severity.Error);
            return;
        }

        if (_userModel.Username.Length < 5 || _userModel.Username.Length > 20)
        {
            Snackbar.Add("Username must be between 5 and 20 characters", Severity.Error);
            return;
        }

        try
        {
            _saving = true;
            List<string> updateMessages = [];

            // Update username if changed
            if (_userModel.Username != _originalUser.UserName)
            {
                await UserManagementProvider.UpdateUsernameAsync(UserId,
                    _userModel.Username,
                    CancellationToken.None);
                updateMessages.Add("username");
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(_userModel.Password))
            {
                await UserManagementProvider.UpdatePasswordAsync(UserId,
                    _userModel.Password,
                    CancellationToken.None);
                updateMessages.Add("password");
            }

            // Update admin status if changed and not current user
            if (_userModel.IsAdmin != _originalUser.IsAdmin
                && !IsCurrentUser())
            {
                await UserManagementProvider.UpdateAdminFlagAsync(UserId,
                    _userModel.IsAdmin,
                    CancellationToken.None);
                updateMessages.Add("admin status");
            }

            if (updateMessages.Any())
            {
                string message = $"User updated successfully ({string.Join(", ", updateMessages)})";
                Snackbar.Add(message, Severity.Success);

                // Reload user data
                await LoadUserAsync();
            }
            else
            {
                Snackbar.Add("No changes to save", Severity.Info);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private async Task ToggleUserStatusAsync()
    {
        if (_originalUser == null || IsCurrentUser())
            return;

        try
        {
            _saving = true;

            if (_originalUser.DisabledAt.HasValue)
            {
                // Enable user
                await UserManagementProvider.EnableUserAsync(UserId,
                    CancellationToken.None);
                Snackbar.Add($"User '{_originalUser.UserName}' has been enabled", Severity.Success);
            }
            else
            {
                // Disable user
                await UserManagementProvider.DisableUserAsync(UserId,
                    CancellationToken.None);
                Snackbar.Add($"User '{_originalUser.UserName}' has been disabled", Severity.Success);
            }

            // Reload user data
            await LoadUserAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user status: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private async Task ShowDeleteConfirmationAsync()
    {
        if (_originalUser == null || IsCurrentUser())
            return;

        bool? result = await DialogService.ShowMessageBox(
            "Delete User",
            $"Are you sure you want to delete user '{_originalUser.UserName}'? This action cannot be undone.",
            yesText: "Delete",
            cancelText: "Cancel");

        if (result == true)
        {
            await DeleteUserAsync();
        }
    }

    private async Task DeleteUserAsync()
    {
        if (_originalUser == null)
            return;

        try
        {
            _saving = true;

            await UserManagementProvider.DeleteUserAsync(UserId,
                CancellationToken.None);

            Snackbar.Add($"User '{_originalUser.UserName}' has been deleted", Severity.Success);
            NavigateBack();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error deleting user: {ex.Message}", Severity.Error);
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
}
