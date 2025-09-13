using Microsoft.Playwright;

namespace HomeBook.E2ETests;

/// <summary>
/// Base test class with Playwright configuration
/// </summary>
[TestFixture]
public abstract class BasePageTest
{
    protected IBrowser Browser { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPlaywright Playwright { get; private set; } = null!;

    /// <summary>
    /// Current browser type being tested
    /// </summary>
    protected PlaywrightConfig.BrowserType CurrentBrowserType { get; private set; }

    /// <summary>
    /// Set up browser, context and page before each test
    /// </summary>
    protected async Task SetUpBrowserAsync(PlaywrightConfig.BrowserType browserType)
    {
        CurrentBrowserType = browserType;

        // Create playwright instance
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Launch browser based on type with specific configurations
        Browser = browserType switch
        {
            PlaywrightConfig.BrowserType.Chromium => await Playwright.Chromium.LaunchAsync(PlaywrightConfig.GetLaunchOptions(browserType)),
            PlaywrightConfig.BrowserType.Firefox => await Playwright.Firefox.LaunchAsync(PlaywrightConfig.GetLaunchOptions(browserType)),
            PlaywrightConfig.BrowserType.WebKit => await LaunchWebKitBrowserAsync(),
            _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
        };

        // Create browser context with options
        Context = await Browser.NewContextAsync(PlaywrightConfig.DefaultContextOptions);

        // Create page
        Page = await Context.NewPageAsync();

        // Configure page with default options
        await Page.SetViewportSizeAsync(1280, 720);

        // Set default timeout for all operations
        Page.SetDefaultTimeout(30000);
        Page.SetDefaultNavigationTimeout(30000);

        // Start tracing for debugging
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.CurrentContext.Test.Name}_{browserType}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    /// <summary>
    /// Launch WebKit browser with macOS-specific configuration
    /// </summary>
    private async Task<IBrowser> LaunchWebKitBrowserAsync()
    {
        BrowserTypeLaunchOptions webkitOptions = new()
        {
            Headless = false, // Explicitly force non-headless for WebKit
            SlowMo = PlaywrightConfig.DefaultLaunchOptions.SlowMo,
            Timeout = PlaywrightConfig.DefaultLaunchOptions.Timeout,
            Args = new[]
            {
                "--disable-web-security"
            }
        };

        Console.WriteLine("Launching WebKit browser with special macOS configuration...");
        IBrowser browser = await Playwright.Webkit.LaunchAsync(webkitOptions);
        Console.WriteLine("WebKit browser launched successfully");

        return browser;
    }

    /// <summary>
    /// Manual cleanup method - called explicitly from ExecuteTestAcrossAllBrowsersAsync
    /// </summary>
    private async Task CleanupBrowserAsync()
    {
        try
        {
            // Save trace on failure
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                string tracePath = Path.Combine("test-results", "traces", $"{TestContext.CurrentContext.Test.Name}_{CurrentBrowserType}_{DateTime.Now:yyyyMMdd-HHmmss}.zip");
                Directory.CreateDirectory(Path.GetDirectoryName(tracePath)!);
                await Context.Tracing.StopAsync(new() { Path = tracePath });

                // Take screenshot on failure
                string screenshotPath = Path.Combine("test-results", "screenshots", $"{TestContext.CurrentContext.Test.Name}_{CurrentBrowserType}_{DateTime.Now:yyyyMMdd-HHmmss}.png");
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            }
            else
            {
                await Context.Tracing.StopAsync();
            }
        }
        catch (Exception ex)
        {
            // Log cleanup error but don't fail the test
            Console.WriteLine($"Warning: Cleanup error in {CurrentBrowserType}: {ex.Message}");
        }
        finally
        {
            // Close page, context, and browser safely
            try { if (Page != null) await Page.CloseAsync(); } catch (Exception ex) { Console.WriteLine($"Page close error: {ex.Message}"); }
            try { if (Context != null) await Context.CloseAsync(); } catch (Exception ex) { Console.WriteLine($"Context close error: {ex.Message}"); }
            try { if (Browser != null) await Browser.CloseAsync(); } catch (Exception ex) { Console.WriteLine($"Browser close error: {ex.Message}"); }
            try { Playwright?.Dispose(); } catch (Exception ex) { Console.WriteLine($"Playwright dispose error: {ex.Message}"); }
        }
    }

    /// <summary>
    /// Empty TearDown to prevent automatic cleanup conflicts
    /// </summary>
    [TearDown]
    public Task TearDownAsync()
    {
        // Do nothing - cleanup is handled manually in ExecuteTestAcrossAllBrowsersAsync
        return Task.CompletedTask;
    }

    /// <summary>
    /// Navigate to a page with default options
    /// </summary>
    protected async Task GotoAsync(string url)
    {
        string fullUrl = url.StartsWith("http") ? url : $"{PlaywrightConfig.BaseUrl}{url}";
        await Page.GotoAsync(fullUrl, PlaywrightConfig.DefaultPageOptions);
    }

    /// <summary>
    /// Wait for an element to be visible and return it
    /// </summary>
    protected async Task<ILocator> WaitForElementAsync(string selector, int timeout = 30000)
    {
        ILocator element = Page.Locator(selector);
        await element.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeout });
        return element;
    }

    /// <summary>
    /// Execute a test method across all configured browsers automatically
    /// </summary>
    protected async Task ExecuteTestAcrossAllBrowsersAsync(Func<Task> testMethod, [System.Runtime.CompilerServices.CallerMemberName] string testName = "")
    {
        foreach (PlaywrightConfig.BrowserType browserType in PlaywrightConfig.EnabledBrowsers)
        {
            Console.WriteLine($"Running {testName} with {browserType} browser...");

            // Setup browser for this iteration
            await SetUpBrowserAsync(browserType);

            try
            {
                await testMethod();
                Console.WriteLine($"✅ {testName} passed with {browserType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {testName} failed with {browserType}: {ex.Message}");
                throw; // Re-throw to fail the test
            }
            finally
            {
                // Cleanup after each browser iteration
                await CleanupBrowserAsync();
            }
        }
    }
}
