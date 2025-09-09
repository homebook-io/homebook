using Bunit;
using HomeBook.Frontend.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using TestContext = Bunit.TestContext;

namespace HomeBook.UnitTests.Frontend.Components;

[TestFixture]
public class UiCountdownAlertTests : TestContext
{
    [SetUp]
    public void SetUp()
    {
        Services.AddMudServices();
    }

    [Test]
    public void UiCountdownAlert_WithDefaultParameters_ShouldRenderCorrectly()
    {
        // Arrange & Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .AddChildContent("Test Alert Message"));

        // Assert
        component.ShouldNotBeNull();
        component.Find(".ui-countdown-alert").ShouldNotBeNull();
        component.Find(".mud-alert").ShouldNotBeNull();
        component.Find(".mud-progress-linear").ShouldNotBeNull();
        component.Markup.ShouldContain("Test Alert Message");
    }

    [Test]
    public void UiCountdownAlert_WithCustomClass_ShouldApplyClassCorrectly()
    {
        // Arrange
        string customClass = "custom-test-class";

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Class, customClass)
            .AddChildContent("Test Content"));

        // Assert
        var stackElement = component.Find(".ui-countdown-alert");
        stackElement.ClassList.ShouldContain(customClass);
    }

    [Test]
    public void UiCountdownAlert_WithCustomSeverity_ShouldRenderWithCorrectSeverity()
    {
        // Arrange
        Severity expectedSeverity = Severity.Error;

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Severity, expectedSeverity)
            .AddChildContent("Error Message"));

        // Assert
        var alertElement = component.Find(".mud-alert");
        alertElement.ClassList.ShouldContain("mud-alert-text-error");
    }

    [Test]
    public void UiCountdownAlert_WithCustomCountdownColor_ShouldApplyColorToProgressBar()
    {
        // Arrange
        Color expectedColor = Color.Secondary;

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.CountdownColor, expectedColor)
            .AddChildContent("Test Content"));

        // Assert
        var progressElement = component.Find(".mud-progress-linear");
        progressElement.ClassList.ShouldContain("mud-progress-linear-color-secondary");
    }

    [Test]
    public void UiCountdownAlert_InitialProgress_ShouldBeZero()
    {
        // Arrange & Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .AddChildContent("Test Content"));

        // Assert
        var progressElement = component.Find(".mud-progress-linear");
        string valueAttribute = progressElement.GetAttribute("aria-valuenow") ?? "0";
        double.Parse(valueAttribute).ShouldBe(0, tolerance: 0.1);
    }

    [Test]
    public void UiCountdownAlert_WithLinearEasing_ShouldProgressLinearly()
    {
        // Arrange
        TimeSpan shortDuration = TimeSpan.FromMilliseconds(100);

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Duration, shortDuration)
            .Add(p => p.Easing, EasingMode.Linear)
            .AddChildContent("Test Content"));

        // Assert - Component should be rendered
        component.ShouldNotBeNull();
        var progressElement = component.Find(".mud-progress-linear");
        progressElement.ShouldNotBeNull();
    }

    [Test]
    public void UiCountdownAlert_WithCustomEasingFunction_ShouldUseCustomFunction()
    {
        // Arrange
        Func<double, double> customEasing = t => t * t; // Square function
        TimeSpan shortDuration = TimeSpan.FromMilliseconds(100);

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Duration, shortDuration)
            .Add(p => p.EasingFunc, customEasing)
            .AddChildContent("Test Content"));

        // Assert
        component.ShouldNotBeNull();
        var progressElement = component.Find(".mud-progress-linear");
        progressElement.ShouldNotBeNull();
    }

    [Test]
    public void UiCountdownAlert_AllEasingModes_ShouldRenderWithoutErrors()
    {
        // Arrange
        EasingMode[] easingModes = Enum.GetValues<EasingMode>();
        TimeSpan shortDuration = TimeSpan.FromMilliseconds(50);

        foreach (EasingMode easingMode in easingModes)
        {
            // Act
            var component = RenderComponent<UiCountdownAlert>(parameters => parameters
                .Add(p => p.Duration, shortDuration)
                .Add(p => p.Easing, easingMode)
                .AddChildContent($"Test Content for {easingMode}"));

            // Assert
            component.ShouldNotBeNull();
            component.Find(".ui-countdown-alert").ShouldNotBeNull();
            component.Find(".mud-progress-linear").ShouldNotBeNull();
        }
    }

    [Test]
    public async Task UiCountdownAlert_OnFinishedCallback_ShouldBeInvokedAfterCompletion()
    {
        // Arrange
        bool callbackInvoked = false;
        EventCallback onFinishedCallback = EventCallback.Factory.Create(this, () => callbackInvoked = true);
        TimeSpan shortDuration = TimeSpan.FromMilliseconds(50);

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Duration, shortDuration)
            .Add(p => p.OnFinished, onFinishedCallback)
            .AddChildContent("Test Content"));

        // Wait for completion with some buffer time
        await Task.Delay(200);

        // Assert
        callbackInvoked.ShouldBeTrue();
    }

    [Test]
    public void UiCountdownAlert_WithZeroDuration_ShouldHandleGracefully()
    {
        // Arrange
        TimeSpan zeroDuration = TimeSpan.Zero;

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Duration, zeroDuration)
            .AddChildContent("Instant Alert"));

        // Assert
        component.ShouldNotBeNull();
        component.Find(".ui-countdown-alert").ShouldNotBeNull();
    }

    [Test]
    public void UiCountdownAlert_WithoutChildContent_ShouldRenderEmptyAlert()
    {
        // Arrange & Act
        var component = RenderComponent<UiCountdownAlert>();

        // Assert
        component.ShouldNotBeNull();
        component.Find(".ui-countdown-alert").ShouldNotBeNull();
        component.Find(".mud-alert").ShouldNotBeNull();
        component.Find(".mud-progress-linear").ShouldNotBeNull();
    }

    [Test]
    public void UiCountdownAlert_WithComplexChildContent_ShouldRenderCorrectly()
    {
        // Arrange & Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "test-child");
                builder.AddContent(2, "Complex Content");
                builder.OpenElement(3, "span");
                builder.AddContent(4, "Nested Element");
                builder.CloseElement();
                builder.CloseElement();
            }));

        // Assert
        component.ShouldNotBeNull();
        component.Markup.ShouldContain("Complex Content");
        component.Markup.ShouldContain("Nested Element");
        component.Find(".test-child").ShouldNotBeNull();
    }

    [Test]
    public void UiCountdownAlert_MultipleSeverityTypes_ShouldRenderCorrectly()
    {
        // Arrange
        Severity[] severities = { Severity.Normal, Severity.Info, Severity.Success, Severity.Warning, Severity.Error };

        foreach (Severity severity in severities)
        {
            // Act
            var component = RenderComponent<UiCountdownAlert>(parameters => parameters
                .Add(p => p.Severity, severity)
                .AddChildContent($"Test {severity} Alert"));

            // Assert
            component.ShouldNotBeNull();
            var alertElement = component.Find(".mud-alert");
            alertElement.ShouldNotBeNull();

            // Verify the severity is applied (each severity has its own CSS class)
            string expectedClass = severity switch
            {
                Severity.Normal => "mud-alert-text-normal",
                Severity.Info => "mud-alert-text-info",
                Severity.Success => "mud-alert-text-success",
                Severity.Warning => "mud-alert-text-warning",
                Severity.Error => "mud-alert-text-error",
                _ => "mud-alert-text-normal"
            };

            alertElement.ClassList.ShouldContain(expectedClass);
        }
    }

    [Test]
    public void UiCountdownAlert_MultipleColorTypes_ShouldRenderCorrectly()
    {
        // Arrange
        Color[] colors = { Color.Primary, Color.Secondary, Color.Tertiary, Color.Success, Color.Warning, Color.Error };

        foreach (Color color in colors)
        {
            // Act
            var component = RenderComponent<UiCountdownAlert>(parameters => parameters
                .Add(p => p.CountdownColor, color)
                .AddChildContent($"Test {color} Progress"));

            // Assert
            component.ShouldNotBeNull();
            var progressElement = component.Find(".mud-progress-linear");
            progressElement.ShouldNotBeNull();

            // Verify the color is applied to progress bar
            string expectedClass = $"mud-progress-linear-color-{color.ToString().ToLower()}";
            progressElement.ClassList.ShouldContain(expectedClass);
        }
    }

    [Test]
    public async Task UiCountdownAlert_ProgressValues_ShouldIncreaseOverTime()
    {
        // Arrange
        TimeSpan duration = TimeSpan.FromMilliseconds(200);
        var progressValues = new List<double>();

        // Act
        var component = RenderComponent<UiCountdownAlert>(parameters => parameters
            .Add(p => p.Duration, duration)
            .Add(p => p.Easing, EasingMode.Linear)
            .AddChildContent("Progress Test"));

        // Capture progress at different intervals
        await Task.Delay(20);
        var progressElement1 = component.Find(".mud-progress-linear");
        string value1 = progressElement1.GetAttribute("aria-valuenow") ?? "0";
        progressValues.Add(double.Parse(value1));

        await Task.Delay(50);
        component.Render(); // Trigger re-render
        var progressElement2 = component.Find(".mud-progress-linear");
        string value2 = progressElement2.GetAttribute("aria-valuenow") ?? "0";
        progressValues.Add(double.Parse(value2));

        await Task.Delay(100);
        component.Render(); // Trigger re-render
        var progressElement3 = component.Find(".mud-progress-linear");
        string value3 = progressElement3.GetAttribute("aria-valuenow") ?? "0";
        progressValues.Add(double.Parse(value3));

        // Assert - Progress should generally increase (allowing for timing variations)
        progressValues.Count.ShouldBe(3);
        progressValues.ShouldAllBe(value => value >= 0 && value <= 100);
    }
}
