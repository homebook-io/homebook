using Microsoft.Playwright;

namespace HomeBook.E2ETests;

/// <summary>
/// Configuration class for Playwright browser settings
/// </summary>
public static class PlaywrightConfig
{
    /// <summary>
    /// Available browser types for testing
    /// </summary>
    public enum BrowserType
    {
        Chromium,
        Firefox,
        WebKit
    }

    /// <summary>
    /// Configured browsers to test - modify this list to change which browsers are tested
    /// </summary>
    public static BrowserType[] EnabledBrowsers =>
    [
        BrowserType.Chromium,
        BrowserType.Firefox,
        BrowserType.WebKit
    ];

    /// <summary>
    /// Default browser launch options
    /// </summary>
    public static BrowserTypeLaunchOptions DefaultLaunchOptions =>
        new()
        {
            Headless = false, // Set to true for CI/CD environments
            SlowMo = 0, // Set to 500 for slower debugging
            Timeout = 30000,
            Args = new[]
            {
                "--disable-web-security",
                "--disable-features=VizDisplayCompositor"
            }
        };

    /// <summary>
    /// Default browser context options
    /// </summary>
    public static BrowserNewContextOptions DefaultContextOptions =>
        new()
        {
            ViewportSize = new ViewportSize
            {
                Width = 1280,
                Height = 720
            },
            IgnoreHTTPSErrors = true,
            RecordVideoDir = "test-results/videos/",
            RecordVideoSize = new RecordVideoSize
            {
                Width = 1280,
                Height = 720
            }
        };

    /// <summary>
    /// Page options for test configuration
    /// </summary>
    public static PageGotoOptions DefaultPageOptions =>
        new()
        {
            Timeout = 30000,
            WaitUntil = WaitUntilState.NetworkIdle
        };

    /// <summary>
    /// Base URL for the application under test
    /// </summary>
    public static string BaseUrl => "http://localhost:5000";

    /// <summary>
    /// Get browser-specific launch options
    /// </summary>
    public static BrowserTypeLaunchOptions GetLaunchOptions(BrowserType browserType)
    {
        BrowserTypeLaunchOptions options = new()
        {
            Headless = DefaultLaunchOptions.Headless,
            SlowMo = DefaultLaunchOptions.SlowMo,
            Timeout = DefaultLaunchOptions.Timeout
        };

        // Browser-specific configurations
        switch (browserType)
        {
            case BrowserType.Chromium:
                options.Args = new[]
                {
                    "--disable-web-security",
                    "--disable-features=VizDisplayCompositor"
                };
                break;
            case BrowserType.Firefox:
                // Firefox specific args if needed
                break;
            case BrowserType.WebKit:
                // WebKit specific configuration for macOS visibility
                options.Args = new[]
                {
                    "--disable-web-security"
                };
                // Force visible mode for WebKit on macOS
                options.Headless = false;
                break;
        }

        return options;
    }
}
