using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightAutoWaits;

/// <summary>
/// Demonstrates Playwright's Expect() API that combines intelligent waiting
/// with assertion logic, eliminating wait-then-assert patterns.
/// </summary>
public class ExpectAPIExamples : PageTest
{
    [Test]
    public async Task ExpectAPI_EliminatesWaitThenAssertPattern()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        // In Selenium: WaitForElementToBeVisible() then Assert.IsTrue()
        // Playwright: Single assertion that waits and verifies
        await Expect(Page.GetByPlaceholder("Username")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Login" })).ToBeEnabledAsync();

        // Fill form and verify state changes
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Assertion waits for navigation and element appearance
        await Expect(Page.GetByText("Products")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ExpectAPI_HandlesTextContentChanges()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Find first "Add to cart" button
        var addButton = Page.GetByText("Add to cart").First;
        await addButton.ClickAsync();

        // Playwright polls until text content changes from "Add to cart" to "Remove"
        await Expect(Page.GetByText("Remove")).ToBeVisibleAsync();

        // Cart badge appears and shows count
        await Expect(Page.Locator(".shopping_cart_badge")).ToHaveTextAsync("1");
    }

    [Test]
    public async Task ExpectAPI_HandlesCountAssertions()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Initially 6 products should be visible
        await Expect(Page.Locator(".inventory_item")).ToHaveCountAsync(6);

        // All "Add to cart" buttons should be present
        await Expect(Page.GetByText("Add to cart")).ToHaveCountAsync(6);

        // Add two items to cart
        await Page.GetByText("Add to cart").First.ClickAsync();
        await Page.GetByText("Add to cart").First.ClickAsync(); // First again since previous changed to "Remove"

        // Now only 4 "Add to cart" buttons remain
        await Expect(Page.GetByText("Add to cart")).ToHaveCountAsync(4);
        // And 2 "Remove" buttons exist
        await Expect(Page.GetByText("Remove")).ToHaveCountAsync(2);
    }

    [Test]
    public async Task AdaptiveWaiting_DemonstratesPerformanceBenefits()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        var startTime = DateTime.Now;

        // Navigate to cart page - Playwright waits optimally
        await Page.GotoAsync("https://www.saucedemo.com/cart.html");

        // These operations complete as soon as elements are ready
        await Expect(Page.Locator("#continue-shopping")).ToBeVisibleAsync();
        await Expect(Page.Locator("#checkout")).ToBeVisibleAsync();

        var executionTime = DateTime.Now - startTime;
        TestContext.WriteLine($"Adaptive waiting completed in {executionTime.TotalMilliseconds}ms");

        // Demonstrate that operations complete quickly when elements are ready
        Assert.That(executionTime.TotalSeconds, Is.LessThan(10),  "Should complete quickly when elements load normally");
    }

    [Test]
    public async Task PerformanceComparison_IntelligentVsConservativeWaiting()
    {
        var testIterations = 3;
        var playwrightTimes = new List<double>();

        for (int i = 0; i < testIterations; i++)
        {
            var startTime = DateTime.Now;

            // Playwright's intelligent waiting - proceeds as soon as ready
            await Page.GotoAsync("https://www.saucedemo.com/");
            await Page.FillAsync("#user-name", "standard_user");
            await Page.FillAsync("#password", "secret_sauce");
            await Page.ClickAsync("#login-button");

            await Expect(Page.GetByText("Products")).ToBeVisibleAsync();
            await Expect(Page.Locator(".inventory_item")).ToHaveCountAsync(6);

            var executionTime = DateTime.Now - startTime;
            playwrightTimes.Add(executionTime.TotalMilliseconds);

            TestContext.WriteLine($"Iteration {i + 1}: {executionTime.TotalMilliseconds}ms");
        }

        var averageTime = playwrightTimes.Average();
        TestContext.WriteLine($"Average execution time with intelligent waiting: {averageTime}ms");

        // Verify consistent performance
        Assert.That(averageTime, Is.LessThan(10000), "Should complete efficiently with intelligent waiting");
    }
}