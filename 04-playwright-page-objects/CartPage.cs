using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace PlaywrightPOM
{
    public class CartPage
    {
        private readonly IPage _page;
        public HeaderComponent Header { get; }
        public ILocator CartItems { get; }
        public ILocator CheckoutButton { get; }

        public CartPage(IPage page)
        {
            _page = page;
            Header = new HeaderComponent(_page);
            CartItems = _page.Locator(".cart_item");
            CheckoutButton = _page.Locator("[data-test='checkout']");
        }

        public async Task ShouldContainItemsAsync(params string[] expectedItemNames)
        {
            await Assertions.Expect(CartItems).ToHaveCountAsync(expectedItemNames.Length);

            foreach (var itemName in expectedItemNames)
            {
                await Assertions.Expect(_page.GetByText(itemName)).ToBeVisibleAsync();
            }
        }

        public async Task<CheckoutPage> ProceedToCheckoutAsync()
        {
            await CheckoutButton.ClickAsync();
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*checkout-step-one.html"));
            return new CheckoutPage(_page);
        }
    }
}
