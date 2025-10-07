using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightLocators.Tests;

public class BasicLocatorExamples : PageTest
{
    [Test]
    public async Task SeleniumStyleLocators_ShowImplementationDependence()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        // Implementation-dependent approach (similar to Selenium CSS selectors)
        // These work but are fragile to design changes
        var usernameFieldOldWay = Page.Locator("#user-name");
        var passwordFieldOldWay = Page.Locator("#password");
        var loginButtonOldWay = Page.Locator("#login-button");

        // Fill login form using implementation-dependent locators
        await usernameFieldOldWay.FillAsync("standard_user");
        await passwordFieldOldWay.FillAsync("secret_sauce");
        await loginButtonOldWay.ClickAsync();

        // Verify login success
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task UserCentricLocators_ShowSemanticApproach()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");

        // User-centric approach - targets how users perceive elements
        // More stable across design changes
        var usernameField = Page.GetByPlaceholder("Username");
        var passwordField = Page.GetByPlaceholder("Password");
        var loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });

        // Fill login form using user-centric locators
        await usernameField.FillAsync("standard_user");
        await passwordField.FillAsync("secret_sauce");
        await loginButton.ClickAsync();

        // Verify login success with semantic assertion
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        await Expect(Page.GetByText("Products")).ToBeVisibleAsync();
    }
}