using Microsoft.Playwright;

namespace HomeBook.E2ETests;

/// <summary>
/// Browser-specific configuration presets
/// </summary>
public static class BrowserPresets
{
    /// <summary>
    /// Development preset - visible browser with slow motion
    /// </summary>
    public static BrowserTypeLaunchOptions Development => new()
    {
        Headless = false,
        SlowMo = 500,
        Timeout = 30000
    };

    /// <summary>
    /// CI/CD preset - headless, fast execution
    /// </summary>
    public static BrowserTypeLaunchOptions Ci => new()
    {
        Headless = true,
        SlowMo = 0,
        Timeout = 30000,
        Args = new[]
        {
            "--disable-gpu",
            "--disable-dev-shm-usage",
            "--no-sandbox"
        }
    };

    /// <summary>
    /// Debug preset - visible browser with debugging tools
    /// </summary>
    public static BrowserTypeLaunchOptions Debug => new()
    {
        Headless = false,
        SlowMo = 1000,
        Timeout = 0, // No timeout for debugging
        Args = new[] { "--start-maximized" }
    };

    /// <summary>
    /// Mobile simulation preset
    /// </summary>
    public static BrowserNewContextOptions Mobile => new()
    {
        ViewportSize = new ViewportSize { Width = 375, Height = 667 },
        DeviceScaleFactor = 2,
        IsMobile = true,
        HasTouch = true,
        UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15"
    };

    /// <summary>
    /// Desktop preset with high resolution
    /// </summary>
    public static BrowserNewContextOptions Desktop => new()
    {
        ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
        DeviceScaleFactor = 1,
        IgnoreHTTPSErrors = true
    };
}
