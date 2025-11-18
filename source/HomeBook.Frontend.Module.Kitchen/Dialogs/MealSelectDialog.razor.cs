using HomeBook.Frontend.Module.Kitchen.Mappings;
using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Module.Kitchen.Dialogs;

public partial class MealSelectDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private List<MealItemViewModel> _mealItems = [];
    private bool _isLoading { get; set; }
    private CancellationTokenSource _cancellationTokenSource = new();
    private Timer? _searchDebounceTimer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        // TODO: load available meals from service
        await SearchAsync(string.Empty);
    }

    private string? _text;

    private void OnSearchTextFieldValueChanged(string? value)
    {
        _text = value;

        // Reset existing timer if any
        _searchDebounceTimer?.Dispose();

        // Create new timer with 3 seconds delay
        long delayMilliseconds = 1000;
        _searchDebounceTimer = new Timer(TimerCallback,
            null,
            TimeSpan.FromMilliseconds(delayMilliseconds),
            Timeout.InfiniteTimeSpan);
    }

    private async void TimerCallback(object? state)
    {
        await InvokeAsync(async () =>
        {
            _searchDebounceTimer = null;

            await SearchAsync(_text ?? string.Empty);
        });
    }

    private async Task SearchAsync(string searchText)
    {
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        _isLoading = true;
        StateHasChanged();

        try
        {
            // Check if cancelled before starting
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<RecipeDto> meals = await RecipeService.GetMealsAsync(searchText,
                cancellationToken);

            _mealItems.Clear();
            _mealItems.AddRange(meals
                .Select(x => x.ToViewModel()));

            StateHasChanged();
        }
        catch (OperationCanceledException)
        {
            // Search was cancelled, do nothing
            return;
        }
        catch (Exception)
        {
            // TODO: display error message
        }
        finally
        {
            // Only update loading state if this operation wasn't cancelled
            if (!cancellationToken.IsCancellationRequested)
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }

    private async Task SelectMealAsync(MealItemViewModel meal)
    {
        await ShutdownAsync();

        MudDialog.Close(DialogResult.Ok(meal));
    }

    private async Task CancelAsync()
    {
        await ShutdownAsync();

        MudDialog.Cancel();
    }

    private async Task ShutdownAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();

        // Cleanup timer
        if (_searchDebounceTimer is not null)
            await _searchDebounceTimer.DisposeAsync();
    }
}
