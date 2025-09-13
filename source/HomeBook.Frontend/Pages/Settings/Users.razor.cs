using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Pages.Settings;

public partial class Users : ComponentBase
{
    [Inject] private BackendClient BackendClient { get; set; } = default!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private bool _loading = true;
    private GetUsersResponse? _usersResponse;
    private List<UserResponse> _users = new();
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;
    private int _totalCount;

    private readonly string[] _pageSizeOptions = { "10", "25", "50", "100" };

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
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

            _usersResponse = await BackendClient.System.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Page = _currentPage;
                requestConfiguration.QueryParameters.PageSize = _pageSize;
                requestConfiguration.Headers.Add("Authorization", $"Bearer {token}");
            });

            if (_usersResponse != null)
            {
                _users = _usersResponse.Users?.ToList() ?? new List<UserResponse>();
                _totalCount = _usersResponse.TotalCount ?? 0;
                _totalPages = _usersResponse.TotalPages ?? 1;
                _currentPage = _usersResponse.Page ?? 1;
                _pageSize = _usersResponse.PageSize ?? 10;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading users: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task OnPageChangedAsync(int page)
    {
        if (page != _currentPage && page > 0 && page <= _totalPages)
        {
            _currentPage = page;
            await LoadUsersAsync();
        }
    }

    private async Task OnPageSizeChangedAsync(string pageSizeString)
    {
        if (int.TryParse(pageSizeString, out int newPageSize) && newPageSize != _pageSize)
        {
            _pageSize = newPageSize;
            _currentPage = 1; // Reset to first page when changing page size
            await LoadUsersAsync();
        }
    }

    private string GetUserStatusText(UserResponse user)
    {
        if (user.Disabled.HasValue)
        {
            return $"Disabled ({user.Disabled.Value:yyyy-MM-dd})";
        }
        return "Active";
    }

    private Color GetUserStatusColor(UserResponse user)
    {
        return user.Disabled.HasValue ? Color.Error : Color.Success;
    }
}
