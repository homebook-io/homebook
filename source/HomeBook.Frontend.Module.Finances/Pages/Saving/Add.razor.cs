using HomeBook.Frontend.Module.Finances.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HomeBook.Frontend.Module.Finances.Pages.Saving;

public partial class Add : ComponentBase
{
    private bool _useQuickAdd = false;
    private int _stepIndex = 0;
    private MudStepper _stepper;
    private MudForm? _formStepName;
    private MudForm? _formStepGoal;
    private bool isStepNameValid = false;
    private bool isStepGoalValid = false;
    private SavingGoalDetailViewModel _model { get; } = new();
    private AddSummaryViewModel? _summaryVM = null;


    private MudForm? _formStepBasicData;
    private MudForm? _formStep2;
    private bool _iconDialogOpen;


    protected readonly List<string> SuggestedIcons =
    [
        Icons.Material.Filled.Savings,
        Icons.Material.Filled.AttachMoney,
        Icons.Material.Filled.TravelExplore,
        Icons.Material.Filled.Home,
        Icons.Material.Filled.Computer
    ];

    protected readonly List<string> AllIcons =
    [
        Icons.Material.Filled.Savings,
        Icons.Material.Filled.AttachMoney,
        Icons.Material.Filled.TravelExplore,
        Icons.Material.Filled.Home,
        Icons.Material.Filled.Computer,
        Icons.Material.Filled.PhoneIphone,
        Icons.Material.Filled.CarRental,
        Icons.Material.Filled.School,
        Icons.Material.Filled.Favorite,
        Icons.Material.Filled.Flight,
        Icons.Material.Filled.Coffee,
        Icons.Material.Filled.Monitor,
        Icons.Material.Filled.FitnessCenter,
        Icons.Material.Filled.Book,
        Icons.Material.Filled.Pets
    ];

    protected async Task NextStep()
    {
        _stepIndex++;

        int maxSteps = _stepper.Steps.Count;
        bool isLast = (_stepIndex + 1) == maxSteps;

        if (!isLast)
            return;

        _summaryVM = new AddSummaryViewModel(_model.Name,
            _model.Color,
            _model.IconName,
            _model.TargetAmount,
            _model.TargetDate
        );
        StateHasChanged();
    }

    protected void PrevStep() => _stepIndex = 0;

    protected void SelectIcon(string icon)
    {
        _model.IconName = icon;
        _iconDialogOpen = false;
    }

    protected async Task SaveAsync()
    {
        await _formStep2!.Validate();
        if (!_formStep2.IsValid)
            return;

        // Beispiel: await SavingGoalService.CreateOrUpdateSavingGoalAsync(Model);
        Snackbar.Add("Sparziel erfolgreich erstellt!", Severity.Success);
    }

    public async Task SwitchToQuickAdd()
    {
        _useQuickAdd = true;
        StateHasChanged();
    }
}
