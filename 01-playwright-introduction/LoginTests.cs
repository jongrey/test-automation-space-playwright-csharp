using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightIntroduction
{
    public class LoginTests : PageTest
    {
        [Test]
        public async Task SuccessfulLogin_ShouldNavigateToProducts()
        {
            // Navigate to the application
            await Page.GotoAsync("https://www.saucedemo.com/");

            // Fill the login form - notice no explicit waits needed
            await Page.FillAsync("#user-name", "standard_user");
            await Page.FillAsync("#password", "secret_sauce");
            await Page.ClickAsync("#login-button");

            // Verify successful navigation
            await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
            await Expect(Page.Locator("[data-test='inventory-container']")).ToBeVisibleAsync();
        }
    }
}