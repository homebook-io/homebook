using HomeBook.Frontend.Module.Finances.Enums;

namespace HomeBook.Frontend.Module.Finances.ViewModels;

public class SavingGoalDetailViewModel
{
    public Guid? Id { get; set; } = null;
    public string Name { get; set; }
    public string Color { get; set; }
    public string IconName { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public InterestRateOptions InterestRateOption { get; set; } = InterestRateOptions.NONE;
    public decimal? InterestRate { get; set; }

    public double Percentage => TargetAmount == 0 ? 0 : (double)(CurrentAmount / TargetAmount);
    public bool IsInterestRateSelected() => InterestRateOption != InterestRateOptions.NONE;
}
