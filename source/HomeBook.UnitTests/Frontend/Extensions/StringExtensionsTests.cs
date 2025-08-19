using HomeBook.Frontend.Extensions;

namespace HomeBook.UnitTests.Frontend.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void ToCssClass_WithValidString_Returns()
    {
        // Arrange
        var input = "TestStringForCssClass";
        var expected = "test-string-for-css-class";

        // Act
        var result = input.ToCssClass();

        // Assert
        result.ShouldBe(expected);
    }
}
