using Microsoft.Playwright;

namespace PlaywrightPOM
{
    public class LoginPage
    {
        // IPage instance - foundation for all page interactions
        private readonly IPage _page;

        // ILocator properties - declarative element definitions
        public ILocator UsernameInput  => _page.GetByPlaceholder("Username");
        public ILocator PasswordInput => _page.GetByPlaceholder("Password");
        public ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        public ILocator ErrorMessage => _page.Locator("[data-test='error']");

        public LoginPage(IPage page)
        {
            _page = page;
        }

        // Navigation method - handles initial page setup
        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://www.saucedemo.com/");

            // Verify the page loaded correctly by checking for key elements
            await Assertions.Expect(UsernameInput).ToBeVisibleAsync();
            await Assertions.Expect(PasswordInput).ToBeVisibleAsync();
            await Assertions.Expect(LoginButton).ToBeVisibleAsync();
        }

        // Service method - represents successful login workflow
        public async Task<ProductsPage> LoginAsStandardUserAsync()
        {
            await UsernameInput.FillAsync("standard_user");
            await PasswordInput.FillAsync("secret_sauce");
            await LoginButton.ClickAsync();

            // Return the next page in the user workflow
            // This enables fluent test chains and provides type safety
            return new ProductsPage(_page);
        }

        // Service method - flexible login for different user types
        public async Task<ProductsPage> LoginAsUserAsync(string username, string password)
        {
            await UsernameInput.FillAsync(username);
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();

            // Wait for successful navigation before returning products page
            await Assertions.Expect(_page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");

            return new ProductsPage(_page);
        }

        // Validation method - verifies page state
        public async Task ShouldDisplayLoginFormAsync()
        {
            await Assertions.Expect(UsernameInput).ToBeVisibleAsync();
            await Assertions.Expect(PasswordInput).ToBeVisibleAsync();
            await Assertions.Expect(LoginButton).ToBeVisibleAsync();
        }
    }
}