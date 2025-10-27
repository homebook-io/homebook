using System.Globalization;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Module.Finances.Enums;
using MudBlazor;

namespace HomeBook.Frontend.Module.Finances.ViewModels;

public class AddSavingGoalSummaryViewModel(IDateTimeProvider DateTime)
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; } = 0;
    public DateTime? TargetDate { get; set; }
    public InterestRateOptions InterestRateOption { get; set; } = InterestRateOptions.NONE;
    public decimal? InterestRate { get; set; }

    public AxisChartOptions AxisChartOptions = new()
    {
        MatchBoundsToSize = true,
    };

    public ChartOptions ChartOptions = new()
    {
        YAxisToStringFunc = value => value.ToString("C"),
        ValueFormatString = "C",
        ShowToolTips = false
    };

    public List<ChartSeries> ChartSeries =>
    [
        new()
        {
            Name = "+Sparplan",
            Data = TargetDate.HasValue
                ? (AmountHistory ?? [])
                .Select(Convert.ToDouble)
                .ToArray()
                : []
        }
    ];

    public string[] YAxisLabels =>
        TargetDate.HasValue
            ? (AmountHistory ?? [])
            .Select(x => x.ToString("C"))
            .ToArray()
            : [];

    public string[] XAxisLabels =>
        TargetDate.HasValue
            ? Enumerable.Range(1, NumberOfMonths.Value)
                .Select(i => DateTime.Now.AddMonths(i).ToString("MMM", CultureInfo.CurrentUICulture))
                .ToArray()
            : [];

    private int? _numberOfMonths;

    public int? NumberOfMonths
    {
        get
        {
            if (_numberOfMonths.HasValue)
                return _numberOfMonths;

            // Fallback to date-based calculation if no target-based calculation was done
            return (TargetDate.HasValue)
                ? ((((DateTime.Now.Year - TargetDate.Value.Year) * 12)
                    + (DateTime.Now.Month - TargetDate.Value.Month))
                   * -1)
                : null;
        }
        private set => _numberOfMonths = value;
    }

    public decimal? AmountPerMonth => (TargetDate.HasValue) ? (TargetAmount / NumberOfMonths) : null;

    public decimal[]? AmountHistory =>
        TargetDate.HasValue
            ? Calculate()
            : [];

    public decimal[] Calculate()
    {
        if (!AmountPerMonth.HasValue || AmountPerMonth.Value <= 0 || TargetAmount <= 0)
            return [];

        List<decimal> result = new();
        decimal currentAmount = 0m;
        int monthsCalculated = 0;
        const int maxMonths = 1000; // Safety limit to prevent infinite loops

        while (currentAmount < TargetAmount && monthsCalculated < maxMonths)
        {
            // Add monthly amount at the beginning of each month
            currentAmount += AmountPerMonth.Value;

            // Apply interest based on InterestRateOption
            if (InterestRateOption != InterestRateOptions.NONE && InterestRate.HasValue)
            {
                decimal interestRateDecimal = InterestRate.Value / 100m;

                if (InterestRateOption == InterestRateOptions.MONTHLY)
                {
                    // Apply monthly interest at the end of each month
                    currentAmount += currentAmount * interestRateDecimal;
                }
                else if (InterestRateOption == InterestRateOptions.YEARLY && (monthsCalculated + 1) % 12 == 0)
                {
                    // Apply yearly interest at the end of each year (every 12 months)
                    currentAmount += currentAmount * interestRateDecimal;
                }
            }

            monthsCalculated++;
            result.Add(currentAmount);
        }

        // Set the calculated number of months
        NumberOfMonths = monthsCalculated;

        return result.ToArray();
    }
}
