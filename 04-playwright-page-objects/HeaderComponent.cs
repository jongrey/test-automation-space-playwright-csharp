using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace PlaywrightPOM
{
    public class HeaderComponent
    {
        private readonly IPage _page;
        private readonly ILocator _container;

        // Component locators are defined relative to the container
        public ILocator ShoppingCartLink { get; }
        public ILocator ShoppingCartBadge { get; }
        public ILocator MenuButton { get; }

        public HeaderComponent(IPage page)
        {
            _page = page;
            // The component's root is the header element
            _container = page.Locator(".primary_header");

            // These locators will only search within the header
            ShoppingCartLink = _container.Locator(".shopping_cart_link");
            ShoppingCartBadge = _container.Locator(".shopping_cart_badge");
            MenuButton = _container.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        }

        // Component service methods encapsulate header-specific workflows
        public async Task<string> GetShoppingCartCountAsync()
        {
            // Handle case where badge is not visible (0 items in cart)
            if (await ShoppingCartBadge.IsVisibleAsync())
            {
                var badgeText = await ShoppingCartBadge.TextContentAsync();
                return badgeText?.Trim() ?? "0";
            }
            return "0";
        }

        public async Task<CartPage> NavigateToCartAsync()
        {
            await ShoppingCartLink.ClickAsync();

            // Verify navigation to cart page
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*cart.html"));

            return new CartPage(_page);
        }

        public async Task OpenMenuAsync()
        {
            await MenuButton.ClickAsync();

            // Wait for menu to appear
            await Assertions.Expect(_page.Locator(".bm-menu")).ToBeVisibleAsync();
        }

        // Validation methods for component state
        public async Task ShouldDisplayCartBadgeAsync(string expectedCount)
        {
            await Assertions.Expect(ShoppingCartBadge).ToHaveTextAsync(expectedCount);
        }

        public async Task ShouldNotDisplayCartBadgeAsync()
        {
            await Assertions.Expect(ShoppingCartBadge).Not.ToBeVisibleAsync();
        }
    }
}