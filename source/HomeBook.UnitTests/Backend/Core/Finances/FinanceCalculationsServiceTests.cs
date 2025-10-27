using System.Globalization;
using HomeBook.Backend.Core.Finances;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.UnitTests.TestCore.Helper;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Core.Finances;

[TestFixture]
public class FinanceCalculationsServiceTests
{
    private CancellationToken _cancellationToken;
    private IDateTimeProvider _dateTimeProvider;
    private FinanceCalculationsService _instance;

    [SetUp]
    public void SetUp()
    {
        _cancellationToken = CancellationToken.None;
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _instance = new FinanceCalculationsService(_dateTimeProvider);
    }

    [Test]
    public void CalculateSavings_WithStraightSumAndTargetSimpleRate_Returns()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        var targetAmount = 1_000;
        var targetDate = now.AddMonths(5);

        // Act
        var result = _instance.CalculateSavings(targetAmount,
            targetDate);

        // Assert
        result.ShouldNotBeNull();
        result.MonthsNeeded.ShouldBe((short)5);
        result.MonthlyPayment.ShouldBe(200);
        result.Amounts.Length.ShouldBe(5);
        result.Interests.Length.ShouldBe(0);

        result.Amounts[0].ShouldBe(200);
        result.Amounts[1].ShouldBe(400);
        result.Amounts[2].ShouldBe(600);
        result.Amounts[3].ShouldBe(800);
        result.Amounts[4].ShouldBe(1_000);
    }

    [Test]
    public void CalculateSavings_WithOddSumAndTargetSimpleRate_Returns()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        var targetAmount = 1_397;
        var targetDate = now.AddMonths(12);

        // Act
        var result = _instance.CalculateSavings(targetAmount,
            targetDate);

        // Assert
        result.ShouldNotBeNull();
        result.MonthsNeeded.ShouldBe((short)12);
        result.MonthlyPayment.ShouldBe(120);
        result.Amounts.Length.ShouldBe(12);
        result.Interests.Length.ShouldBe(0);

        result.Amounts[0].ShouldBe(120);
        result.Amounts[1].ShouldBe(240);
        result.Amounts[2].ShouldBe(360);
        result.Amounts[3].ShouldBe(480);
        result.Amounts[4].ShouldBe(600);
        result.Amounts[5].ShouldBe(720);
        result.Amounts[6].ShouldBe(840);
        result.Amounts[7].ShouldBe(960);
        result.Amounts[8].ShouldBe(1_080);
        result.Amounts[9].ShouldBe(1_200);
        result.Amounts[10].ShouldBe(1_320);
        result.Amounts[11].ShouldBe(1_440);
    }

    [Test]
    public void CalculateSavings_WithOddSumAndWithoutTargetSimpleRate_Returns()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        var targetAmount = 1_397;
        var targetDate = now.AddMonths(12);

        // Act
        var result = _instance.CalculateSavings(targetAmount,
            targetDate,
            false);

        // Assert
        result.ShouldNotBeNull();
        result.MonthsNeeded.ShouldBe((short)12);
        result.MonthlyPayment.ShouldBe((decimal)116.4, 1);
        result.Amounts.Length.ShouldBe(12);
        result.Interests.Length.ShouldBe(0);

        result.Amounts[0].ShouldBe((decimal)116.4, 1);
        result.Amounts[1].ShouldBe((decimal)232.8, 1);
        result.Amounts[2].ShouldBe((decimal)349.2, 1);
        result.Amounts[3].ShouldBe((decimal)465.6, 1);
        result.Amounts[4].ShouldBe((decimal)582.0, 1);
        result.Amounts[5].ShouldBe((decimal)698.5, 1);
        result.Amounts[6].ShouldBe((decimal)814.9, 1);
        result.Amounts[7].ShouldBe((decimal)931.3, 1);
        result.Amounts[8].ShouldBe((decimal)1047.7, 1);
        result.Amounts[9].ShouldBe((decimal)1164.1, 1);
        result.Amounts[10].ShouldBe((decimal)1280.5, 1);
        result.Amounts[11].ShouldBe((decimal)1397.0, 1);
    }

    [TestCase(7_000,
        18,
        2.08,
        18,
        18,
        1,
        1,
        "331.76,670.42,1016.13,1369.02,1729.26,2096.98,2472.36,2855.55,3246.70,3645.99,4053.59,4469.67,4894.39,5327.96,5770.54,6222.33,6683.51,7154.29",
        "6.76,13.66,20.70,27.90,35.24,42.73,50.38,58.19,66.16,74.29,82.60,91.07,99.73,108.56,117.58,126.79,136.18,145.78")]
    [TestCase(1_000,
        6,
        0,
        6,
        6,
        1,
        1,
        "170,340,510,680,850,1020",
        "0,0,0,0,0,0")]
    [TestCase(1_200,
        12,
        0.5,
        12,
        12,
        1,
        1,
        "100.50,201.50,303.01,405.03,507.55,610.59,714.14,818.21,922.80,1027.92,1133.56,1239.72",
        "0.50,1.00,1.51,2.02,2.53,3.04,3.55,4.07,4.59,5.11,5.64,6.17")]
    [TestCase(5_000,
        6,
        1.2,
        6,
        6,
        1,
        1,
        "819.72,1649.28,2488.79,3338.37,4198.15,5068.25",
        "9.72,19.56,29.51,39.59,49.78,60.10")]
    [TestCase(10_000,
        6,
        0.3,
        6,
        6,
        1,
        1,
        "1659.96,3324.91,4994.85,6669.80,8349.77,10034.79",
        "4.96,9.94,14.94,19.95,24.97,30.01")]
    [TestCase(20_000,
        6,
        0.15,
        6,
        6,
        1,
        1,
        "3329.99,6664.97,10004.95,13349.95,16699.96,20055.00",
        "4.99,9.98,14.98,19.99,25.01,30.04")]
    public void CalculateMonthlySavings_WithDifferentSettings_Returns(decimal targetAmount,
        int targetAdditionalMonthsFromNow,
        decimal interestRate,
        short expectedMonthsNeeded,
        int expectedListLength,
        decimal amountExpectedTolerance,
        decimal interestsExpectedTolerance,
        string expectedAmountsCsv,
        string expectedInterestsCsv)
    {
        // Arrange
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        var targetDate = now.AddMonths(targetAdditionalMonthsFromNow);
        decimal[] expectedAmounts = expectedAmountsCsv.ToDecimalArray(new CultureInfo("en-US"));
        decimal[] expectedInterests = expectedInterestsCsv.ToDecimalArray(new CultureInfo("en-US"));

        // Act
        var result = _instance.CalculateMonthlySavings(targetAmount,
            targetDate,
            interestRate);

        // Assert
        result.ShouldNotBeNull();
        string amountCsv =
            string.Join(",", result.Amounts.Select(a => a.ToString(CultureInfo.GetCultureInfo("en-US"))));
        string interestCsv = string.Join(",",
            result.Interests.Select(i => i.ToString(CultureInfo.GetCultureInfo("en-US"))));
        result.MonthsNeeded.ShouldBe(expectedMonthsNeeded);
        result.Amounts.Length.ShouldBe(expectedListLength);
        result.Interests.Length.ShouldBe(expectedListLength);

        result.Amounts.ShouldBe(expectedAmounts, amountExpectedTolerance);
        result.Interests.ShouldBe(expectedInterests, interestsExpectedTolerance);
    }

    [TestCase(1_000,
        18,
        0,
        6,
        6,
        1,
        1,
        "0,0",
        "0,0")]
    [TestCase(5_000,
        60,
        2.0,
        12,
        12,
        1,
        1,
        "0,0",
        "0,0")]
    [TestCase(10_000,
        120,
        3.5,
        12,
        12,
        1,
        1,
        "0,0",
        "0,0")]
    [TestCase(20_000,
        180,
        4.2,
        12,
        12,
        1,
        1,
        "0,0",
        "0,0")]
    [TestCase(100_000,
        240,
        5.0,
        12,
        12,
        1,
        1,
        "0,0",
        "0,0")]
    public void CalculateYearlySavings_WithDifferentSettings_Returns(decimal targetAmount,
        int targetAdditionalMonthsFromNow,
        decimal interestRate,
        short expectedMonthsNeeded,
        int expectedListLength,
        decimal amountExpectedTolerance,
        decimal interestsExpectedTolerance,
        string expectedAmountsCsv,
        string expectedInterestsCsv)
    {
        // Arrange
        var now = DateTime.UtcNow;
        _dateTimeProvider.UtcNow.Returns(now);
        var targetDate = now.AddMonths(targetAdditionalMonthsFromNow);
        decimal[] expectedAmounts = expectedAmountsCsv.ToDecimalArray(new CultureInfo("en-US"));
        decimal[] expectedInterests = expectedInterestsCsv.ToDecimalArray(new CultureInfo("en-US"));

        // Act
        var result = _instance.CalculateYearlySavings(targetAmount,
            targetDate,
            interestRate);

        // Assert
        result.ShouldNotBeNull();
        result.MonthsNeeded.ShouldBe(expectedMonthsNeeded);
        result.Amounts.Length.ShouldBe(expectedListLength);
        result.Interests.Length.ShouldBe(expectedListLength);

        result.Amounts.ShouldBe(expectedAmounts, amountExpectedTolerance);
        result.Interests.ShouldBe(expectedInterests, interestsExpectedTolerance);
    }
}
