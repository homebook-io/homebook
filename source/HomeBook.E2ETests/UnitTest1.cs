using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace HomeBook.E2ETests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : BasePageTest
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await ExecuteTestAcrossAllBrowsersAsync(async () =>
        {
            await GotoAsync("https://playwright.dev");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

            // Create a locator
            ILocator getStarted = Page.Locator("text=Get Started");

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

            // Click the get started link.
            await getStarted.ClickAsync();

            // Expects the URL to contain intro.
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        });
    }
}
