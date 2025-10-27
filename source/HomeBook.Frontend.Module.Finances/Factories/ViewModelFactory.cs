using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Module.Finances.Contracts;
using HomeBook.Frontend.Module.Finances.ViewModels;

namespace HomeBook.Frontend.Module.Finances.Factories;

/// <inheritdoc/>
public class ViewModelFactory(IDateTimeProvider dateTimeProvider) : IViewModelFactory
{
    /// <inheritdoc/>
    public AddSavingGoalSummaryViewModel CreateAddSavingGoalSummaryViewModel() => new(dateTimeProvider);
}
