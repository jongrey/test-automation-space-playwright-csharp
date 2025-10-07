using Microsoft.Playwright.NUnit;

namespace PlaywrightAutoWaits;

/// <summary>
/// Demonstrates Playwright's built-in actionability checks that eliminate
/// the need for explicit wait wrapper methods you built in Selenium.
/// </summary>
public class AutoWaitDemonstration : PageTest
{
    [Test]
    public async Task AutoWait_HandlesBasicInteractions_WithoutExplicitWaits()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        // In Selenium: WaitForElementToBeClickable(loginButton)
        // Playwright: Automatic actionability checks handle this
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Auto-waits for page navigation and element visibility
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        await Expect(Page.GetByText("Products")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AutoWait_HandlesDynamicContent_InProductInteraction()
    {
        // Login first
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Find and interact with product - auto-waits handle timing
        var addToCartButton = Page.GetByText("Add to cart").First;
        await addToCartButton.ClickAsync();

        // Playwright automatically waits for the button text to change
        await Expect(Page.GetByText("Remove")).ToBeVisibleAsync();

        // Shopping cart badge updates dynamically
        await Expect(Page.Locator(".shopping_cart_badge")).ToHaveTextAsync("1");
    }
}