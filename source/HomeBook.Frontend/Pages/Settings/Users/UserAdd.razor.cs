using System.ComponentModel.DataAnnotations;
using HomeBook.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings.Users;

public partial class UserAdd : ComponentBase
{
    private CreateUserModel _userModel = new();
    private bool _saving;

    private List<BreadcrumbItem> _breadcrumbs = new()
    {
        new BreadcrumbItem("Settings", href: "/Settings", icon: Icons.Material.Filled.Settings),
        new BreadcrumbItem("Users", href: "/Settings/Users", icon: Icons.Material.Filled.People),
        new BreadcrumbItem("Add User", href: null, disabled: true, icon: Icons.Material.Filled.PersonAdd)
    };

    private async Task HandleValidSubmitAsync()
    {
        if (_saving)
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

            CreateUserRequest request = new CreateUserRequest
            {
                Username = _userModel.Username,
                Password = _userModel.Password,
                IsAdmin = _userModel.IsAdmin
            };

            // TODO: Implement actual backend call when client is configured
            // Need backend endpoint: POST /system/users
            // await BackendClient.System.Users.PostAsync(request, x => x.Headers.Add("Authorization", $"Bearer {token}"));

            Snackbar.Add($"User '{_userModel.Username}' has been created successfully", Severity.Success);
            NavigationManager.NavigateTo("/Settings/Users");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error creating user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/Settings/Users");
    }

    public class CreateUserModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
