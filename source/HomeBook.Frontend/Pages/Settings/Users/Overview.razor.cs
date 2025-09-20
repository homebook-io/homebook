using System.Security.Claims;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Abstractions.Models.System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings.Users;

public partial class Overview : ComponentBase
{
    private bool _loading = true;
    private List<UserData> _users = new();
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalCount;
    private int _totalPages;

    private readonly string[] _pageSizeOptions =
    [
        "5",
        "10",
        "25",
        "50"
    ];

    private Guid? _currentUserId;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUserAsync();
        await LoadUsersAsync();
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

    private async Task LoadUsersAsync()
    {
        try
        {
            _loading = true;
            PagedList<UserData>? response =
                await UserManagementProvider.GetAllUsersAsync((ushort)_currentPage,
                    (ushort)_pageSize,
                    CancellationToken.None);
            if (response is not null)
            {
                _users = response.Items.ToList();
                _totalCount = response.TotalCount;
                _totalPages = response.TotalPages;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading users: {ex.Message}", Severity.Error);
            _users = [];
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnPageChangedAsync(int page)
    {
        _currentPage = page;
        await LoadUsersAsync();
    }

    private async Task OnPageSizeChangedAsync(string pageSizeString)
    {
        if (int.TryParse(pageSizeString, out int newPageSize))
        {
            _pageSize = newPageSize;
            _currentPage = 1; // Reset to first page when changing page size
            await LoadUsersAsync();
        }
    }

    private void NavigateToAddUser()
    {
        NavigationManager.NavigateTo("/Settings/Users/Add");
    }

    private void NavigateToEditUser(Guid userId)
    {
        NavigationManager.NavigateTo($"/Settings/Users/{userId}");
    }

    private bool IsCurrentUser(Guid userId)
    {
        return _currentUserId.HasValue && _currentUserId.Value == userId;
    }

    private async Task ToggleUserStatusAsync(UserData user)
    {
        if (IsCurrentUser(user.Id))
        {
            Snackbar.Add("You cannot modify your own account status", Severity.Warning);
            return;
        }

        try
        {
            if (user.DisabledAt.HasValue)
            {
                // Enable user
                await UserManagementProvider.EnableUserAsync(user.Id, CancellationToken.None);
                Snackbar.Add($"User '{user.UserName}' has been enabled", Severity.Success);
            }
            else
            {
                // Disable user
                await UserManagementProvider.DisableUserAsync(user.Id, CancellationToken.None);
                Snackbar.Add($"User '{user.UserName}' has been disabled", Severity.Success);
            }

            // Reload users to reflect the changes
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating user status: {ex.Message}", Severity.Error);
        }
    }

    private Color GetUserStatusColor(UserData user)
    {
        return user.DisabledAt.HasValue ? Color.Error : Color.Success;
    }

    private string GetUserStatusText(UserData user)
    {
        return user.DisabledAt.HasValue ? "Disabled" : "Active";
    }
}
