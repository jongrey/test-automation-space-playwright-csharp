using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightAutoWaits;

/// <summary>
/// Demonstrates Playwright's five actionability checks that run automatically:
/// 1. Attached - Element exists in DOM
/// 2. Visible - Element is visible to users
/// 3. Stable - Element is not animating
/// 4. Receives Events - Element can receive interactions
/// 5. Enabled - Form controls are enabled
/// </summary>
public class ActionabilityChecksDemo : PageTest
{
    [Test]
    public async Task ActionabilityChecks_HandleAllConditionsAutomatically()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        // Playwright automatically verifies ALL actionability conditions:
        // ✓ Element attached to DOM
        // ✓ Element visible (not display:none, opacity:0, etc.)
        // ✓ Element stable (not moving/animating)
        // ✓ Element can receive events (not covered by other elements)
        // ✓ Element enabled (for form controls)

        var usernameField = Page.GetByPlaceholder("Username");
        var passwordField = Page.GetByPlaceholder("Password");
        var loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });

        // Each interaction waits for all conditions automatically
        await usernameField.FillAsync("standard_user");
        await passwordField.FillAsync("secret_sauce");
        await loginButton.ClickAsync();

        // Post-login elements are also verified for actionability
        var inventoryContainer = Page.Locator("[data-test='inventory-container']");
        await Expect(inventoryContainer).ToBeVisibleAsync();

        TestContext.WriteLine("All interactions succeeded with built-in actionability verification");
    }

    [Test]
    public async Task WhenCustomWaitsAreStillNeeded()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        // Auto-waits handle element interactions, but NOT application-specific logic
        // Example: Waiting for specific business state transitions

        // Add item to cart
        await Page.GetByText("Add to cart").First.ClickAsync();

        // Navigate to cart
        await Page.ClickAsync(".shopping_cart_link");

        // Custom wait example: Wait for cart to show "processing" state
        // (This is hypothetical - saucedemo doesn't have this, but real apps do)
        // You might need: await Expect(cartStatus).ToHaveTextAsync("Processing order...");

        // For now, just verify cart contents
        await Expect(Page.Locator(".cart_item")).ToHaveCountAsync(1);

        TestContext.WriteLine("Auto-waits handle UI interactions, custom waits handle business logic");
    }
}