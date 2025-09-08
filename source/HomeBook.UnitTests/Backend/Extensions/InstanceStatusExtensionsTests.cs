using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Extensions;

namespace HomeBook.UnitTests.Backend.Extensions;

public class InstanceStatusExtensionsTests
{
    [TestCase("", InstanceStatus.SETUP)]
    [TestCase(null, InstanceStatus.SETUP)]
    [TestCase("not-empty-value", InstanceStatus.RUNNING)]
    public void ContainsUserAsync_ShouldReturnExpectedResult(string? status, InstanceStatus expected)
    {
        // Arrange
        var configuration = new DataBuilder()
            .Add("Database", new DataBuilder()
                    .Add("Provider", status))
            .ToConfiguration();

        // Act
        var actual = configuration.GetCurrentInstanceStatus();

        // Assert
        actual.ShouldBe(expected);
    }
}
