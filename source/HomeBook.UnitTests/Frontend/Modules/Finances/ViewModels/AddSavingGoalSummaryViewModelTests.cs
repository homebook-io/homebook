using System.Globalization;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Module.Finances.Enums;
using HomeBook.Frontend.Module.Finances.Factories;
using NSubstitute;

namespace HomeBook.UnitTests.Frontend.Modules.Finances.ViewModels;

[TestFixture]
public class AddSavingGoalSummaryViewModelTests
{
    [Test]
    public void Calculate_WithTargetDateAndMonthly_Returns()
    {
        // Arrange
        DateTime now = DateTime.Parse("2025-10-24");
        DateTime targetDate = now.AddMonths(10).AddDays(5);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Now.Returns(now);

        var vmFactory = new ViewModelFactory(dateTimeProvider);
        var instance = vmFactory.CreateAddSavingGoalSummaryViewModel();
        instance.TargetAmount = 10_000;
        instance.TargetDate = targetDate;
        instance.InterestRateOption = InterestRateOptions.MONTHLY;
        instance.InterestRate = 2;

        // Act
        var result = instance.Calculate()
            .Select(x => x.ToString("C", new CultureInfo("de-DE")))
            .ToArray();

        // Assert
        result.Length.ShouldBe(10);

        result[0].ShouldBe("1.020,00 €");
        result[1].ShouldBe("2.060,40 €");
        result[2].ShouldBe("3.121,61 €");
        result[3].ShouldBe("4.204,04 €");
        result[4].ShouldBe("5.308,12 €");
        result[5].ShouldBe("6.434,28 €");
        result[6].ShouldBe("7.582,97 €");
        result[7].ShouldBe("8.754,63 €");
        result[8].ShouldBe("9.949,72 €");
        result[9].ShouldBe("11.168,72 €");
    }

    [Test]
    public void Calculate_WithTargetDateAndYearly_Returns()
    {
        // Arrange
        DateTime now = DateTime.Parse("2025-10-24");
        DateTime targetDate = now.AddYears(5).AddMonths(10).AddDays(5);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Now.Returns(now);

        var vmFactory = new ViewModelFactory(dateTimeProvider);
        var instance = vmFactory.CreateAddSavingGoalSummaryViewModel();
        instance.TargetAmount = 10_000;
        instance.TargetDate = targetDate;
        instance.InterestRateOption = InterestRateOptions.YEARLY;
        instance.InterestRate = 2;

        // Act
        var result = instance.Calculate()
            .Select(x => x.ToString("C", new CultureInfo("de-DE")))
            .ToArray();

        // Assert
        result.Length.ShouldBe(70);

        result[0].ShouldBe("142,86 €");
        result[6].ShouldBe("1.000,00 €");
        result[10].ShouldBe("1.571,43 €");
        result[20].ShouldBe("3.034,29 €");
        result[30].ShouldBe("4.532,11 €");
        result[40].ShouldBe("6.065,61 €");
        result[50].ShouldBe("7.635,50 €");
        result[60].ShouldBe("9.242,49 €");
        result[69].ShouldBe("10.528,21 €");
    }
}
