using System.Globalization;
using MudBlazor;

namespace HomeBook.Frontend.Module.Finances.ViewModels;

public class AddSummaryViewModel(
    string name,
    string color,
    string iconName,
    decimal amount,
    DateTime? date)
{
    public string Name { get; set; } = name;
    public string Color { get; set; } = color;
    public string IconName { get; set; } = iconName;
    public decimal TargetAmount { get; set; } = amount;
    public DateTime? TargetDate { get; set; } = date;

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
                ? AmountHistory
                    .Select(Convert.ToDouble)
                    .ToArray()
                : []
        }
    ];

    public string[] YAxisLabels =>
        TargetDate.HasValue
            ? AmountHistory
                .Select(x => x.ToString("C"))
                .ToArray()
            : [];

    public string[] XAxisLabels =>
        TargetDate.HasValue
            ? Enumerable.Range(1, NumberOfMonths.Value)
                .Select(i => DateTime.Now.AddMonths(i).ToString("MMM", CultureInfo.CurrentUICulture))
                .ToArray()
            : [];

    public int? NumberOfMonths =>
        (TargetDate.HasValue)
            ? ((((DateTime.Now.Year - TargetDate.Value.Year) * 12) + (DateTime.Now.Month - TargetDate.Value.Month))
               * -1)
            : null;

    public decimal? AmountPerMonth => (TargetDate.HasValue) ? (TargetAmount / NumberOfMonths) : null;

    public decimal[]? AmountHistory =>
        TargetDate.HasValue
            ? Enumerable.Range(1, NumberOfMonths.Value)
                .Select(i => (i * AmountPerMonth.Value))
                .ToArray()
            : [];
}
