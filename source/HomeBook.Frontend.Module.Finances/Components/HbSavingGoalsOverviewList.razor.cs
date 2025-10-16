using HomeBook.Frontend.Module.Finances.Models;
using HomeBook.Frontend.Module.Finances.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Finances.Components;

public partial class HbSavingGoalsOverviewList : ComponentBase
{
    private List<SavingGoalOverviewViewModel> _savingGoals = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        CancellationToken cancellationToken = CancellationToken.None;
        await LoadSavingGoalsAsync(cancellationToken);
    }

    private async Task LoadSavingGoalsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<SavingGoal> savingGoals = await SavingGoalService.GetAllSavingGoalsAsync(cancellationToken);
        _savingGoals = savingGoals.Select(sg => new SavingGoalOverviewViewModel(
                sg.Id,
                sg.Name,
                sg.Color,
                sg.TargetAmount,
                sg.CurrentAmount,
                sg.TargetDate,
                sg.Percentage))
            .ToList();
        StateHasChanged();
    }
}
