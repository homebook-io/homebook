using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HomeBook.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings.Users;

public partial class UserEdit : ComponentBase
{
    [Parameter]
    public Guid UserId { get; set; }

    private EditUserModel? _userModel;
    private UserResponse? _originalUser;
    private bool _loading = true;
    private bool _saving;
    private Guid? _currentUserId;

    private List<BreadcrumbItem> _breadcrumbs = new()
    {
        new BreadcrumbItem("Settings", href: "/Settings", icon: Icons.Material.Filled.Settings),
        new BreadcrumbItem("Users", href: "/Settings/Users", icon: Icons.Material.Filled.People),
        new BreadcrumbItem("Edit User", href: null, disabled: true, icon: Icons.Material.Filled.Edit)
    };

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentUserIdAsync();
        await LoadUserAsync();
    }

    private async Task GetCurrentUserIdAsync()
    {
        try
        {
            ClaimsPrincipal currentUser = await AuthenticationService.GetCurrentUserAsync();
            string? userIdClaim = currentUser?.FindFirst("sub")?.Value
                                  ?? currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _currentUserId = userId;
            }
        }
        catch
        {
            _currentUserId = null;
        }
    }

    private async Task LoadUserAsync()
    {
        try
        {
            _loading = true;
            StateHasChanged();

            string? token = await AuthenticationService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Snackbar.Add("Authentication token not found", Severity.Error);
                return;
            }

            // TODO: Implement actual backend call when client is configured
            // Need backend endpoint: GET /system/users/{userId}
            // UserResponse user = await BackendClient.System.Users[UserId].GetAsync(x => x.Headers.Add("Authorization", $"Bearer {token}"));

            // Mock user data for now - this should be replaced with actual backend call
            UserResponse user = new()
            {
                Id = UserId,
                Username = "MockUser",
                Created = DateTimeOffset.Now,
                Disabled = null,
                IsAdmin = false
            };

            if (user != null)
            {
                _originalUser = user;
                _userModel = new EditUserModel
                {
                    Username = user.Username ?? string.Empty,
                    Password = string.Empty, // Never populate password field
                    IsAdmin = user.IsAdmin == true
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
            StateHasChanged();
        }
    }

    private async Task HandleValidSubmitAsync()
    {
        if (_saving || _userModel == null)
            return;

        try
        {
            _saving = true;
            StateHasChanged();

            string? token = await AuthenticationService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Snackbar.Add("Authentication token not found", Severity.Error);
                return;
            }

            // UpdateUserRequest request = new UpdateUserRequest
            // {
            //     Username = _userModel.Username,
            //     IsAdmin = _userModel.IsAdmin
            // };

            // Only include password if it was changed
            // if (!string.IsNullOrWhiteSpace(_userModel.Password))
            // {
            //     request.Password = _userModel.Password;
            // }

            // TODO: Implement actual backend call when client is configured
            // Need backend endpoint: PUT /system/users/{userId}
            // await BackendClient.System.Users[UserId].PutAsync(request, x => x.Headers.Add("Authorization", $"Bearer {token}"));

            Snackbar.Add($"User '{_userModel.Username}' has been updated successfully", Severity.Success);
            await LoadUserAsync(); // Refresh user data
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }
    }

    private async Task ToggleUserStatusAsync()
    {
        if (IsCurrentUser() || _originalUser == null)
        {
            Snackbar.Add("Cannot modify your own account status", Severity.Warning);
            return;
        }

        try
        {
            _saving = true;
            StateHasChanged();

            string? token = await AuthenticationService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Snackbar.Add("Authentication token not found", Severity.Error);
                return;
            }

            if (_originalUser.Disabled.HasValue)
            {
                // TODO: Implement actual backend calls when client is configured
                // Need backend endpoint: PUT /system/users/{userId}/enable
                // await BackendClient.System.Users[UserId].EnableAsync(x => x.Headers.Add("Authorization", $"Bearer {token}"));
                Snackbar.Add($"User '{_originalUser.Username}' has been enabled", Severity.Success);
            }
            else
            {
                // TODO: Implement actual backend calls when client is configured
                // Need backend endpoint: PUT /system/users/{userId}/disable
                // await BackendClient.System.Users[UserId].DisableAsync(x => x.Headers.Add("Authorization", $"Bearer {token}"));
                Snackbar.Add($"User '{_originalUser.Username}' has been disabled", Severity.Success);
            }

            await LoadUserAsync(); // Refresh user data
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user status: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }
    }

    private async Task ShowDeleteConfirmationAsync()
    {
        if (IsCurrentUser() || _originalUser == null)
        {
            Snackbar.Add("Cannot delete your own account", Severity.Warning);
            return;
        }

        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete user '{_originalUser.Username}'? This action cannot be undone.",
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
            StateHasChanged();

            string? token = await AuthenticationService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Snackbar.Add("Authentication token not found", Severity.Error);
                return;
            }

            DeleteUserRequest request = new DeleteUserRequest
            {
                UserId = UserId
            };

            // TODO: Implement actual backend call when client is configured
            // Need backend endpoint: DELETE /system/users
            // await BackendClient.System.Users.DeleteAsync(request, x => x.Headers.Add("Authorization", $"Bearer {token}"));

            Snackbar.Add($"User '{_originalUser.Username}' has been deleted", Severity.Success);
            NavigationManager.NavigateTo("/Settings/Users");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error deleting user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }
    }

    private bool IsCurrentUser()
    {
        return _currentUserId.HasValue && _currentUserId == UserId;
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/Settings/Users");
    }

    public class EditUserModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
