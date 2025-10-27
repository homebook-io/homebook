using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Frontend.Abstractions.Contracts;

namespace HomeBook.Backend.Core.Finances;

/// <inheritdoc/>
public class FinanceCalculationsService(IDateTimeProvider dateTimeProvider)
    : IFinanceCalculationsService
{
    public SavingCalculationResult CalculateSavings(decimal targetAmount,
        DateTime targetDate,
        bool targetSimpleRate = true)
    {
        var now = dateTimeProvider.UtcNow;
        var totalMonths = ((targetDate.Year - now.Year) * 12) + targetDate.Month - now.Month;
        if (totalMonths <= 0)
            return new SavingCalculationResult(0,
                0,
                [],
                []);

        var monthlyPayment = targetAmount / totalMonths;

        // Wenn einfache Rate aktiviert ist: auf den nächsten 5er-Schritt aufrunden
        if (targetSimpleRate)
        {
            monthlyPayment = Math.Ceiling(monthlyPayment / 5) * 5;
        }

        var amounts = new decimal[totalMonths];
        for (var monthIndex = 0; monthIndex < totalMonths; monthIndex++)
        {
            amounts[monthIndex] = monthlyPayment * (monthIndex + 1);
        }

        return new SavingCalculationResult((short)totalMonths,
            monthlyPayment,
            amounts,
            []);
    }

    /// <inheritdoc/>
    public SavingCalculationResult CalculateMonthlySavings(decimal targetAmount,
        DateTime targetDate,
        decimal interestRate,
        bool targetSimpleRate = true)
    {
        var now = dateTimeProvider.UtcNow;
        var totalMonths = ((targetDate.Year - now.Year) * 12) + targetDate.Month - now.Month;
        if (totalMonths <= 0)
            return new SavingCalculationResult(0, 0, [], []);

        var monthlyRate = interestRate / 100m;

        decimal monthlyPayment;

        if (monthlyRate == 0)
        {
            // Kein Zins → einfache lineare Aufteilung
            monthlyPayment = targetAmount / totalMonths;
        }
        else
        {
            // Zinseszinsformel
            var divisor = ((decimal)Math.Pow((double)(1 + monthlyRate), totalMonths) - 1) / monthlyRate;
            monthlyPayment = targetAmount / divisor;
        }

        if (targetSimpleRate)
            monthlyPayment = Math.Ceiling(monthlyPayment / 5) * 5;

        var amounts = new decimal[totalMonths];
        var interests = new decimal[totalMonths];
        decimal balance = 0;

        for (var i = 0; i < totalMonths; i++)
        {
            balance += monthlyPayment;
            var interest = balance * monthlyRate;
            balance += interest;

            interests[i] = Math.Round(interest, 2);
            amounts[i] = Math.Round(balance, 2);
        }

        return new SavingCalculationResult((short)totalMonths,
            monthlyPayment,
            amounts,
            interests);
    }

    /// <inheritdoc/>
    public SavingCalculationResult CalculateYearlySavings(decimal targetAmount,
        DateTime targetDate,
        decimal interestRate,
        bool targetSimpleRate = true)
    {
        var now = dateTimeProvider.UtcNow;
        var totalYears = targetDate.Year - now.Year;
        if (totalYears <= 0)
            return new SavingCalculationResult(0, 0, [], []);

        var yearlyRate = interestRate / 100m;

        decimal yearlyPayment;

        if (yearlyRate == 0)
        {
            yearlyPayment = targetAmount / totalYears;
        }
        else
        {
            var divisor = ((decimal)Math.Pow((double)(1 + yearlyRate), totalYears) - 1) / yearlyRate;
            yearlyPayment = targetAmount / divisor;
        }

        if (targetSimpleRate)
            yearlyPayment = Math.Ceiling(yearlyPayment / 5) * 5;

        var amounts = new decimal[totalYears];
        var interests = new decimal[totalYears];
        decimal balance = 0;

        for (var i = 0; i < totalYears; i++)
        {
            balance += yearlyPayment;
            var interest = balance * yearlyRate;
            balance += interest;

            interests[i] = Math.Round(interest, 2);
            amounts[i] = Math.Round(balance, 2);
        }

        return new SavingCalculationResult((short)totalYears,
            yearlyPayment,
            amounts,
            interests);
    }
}
