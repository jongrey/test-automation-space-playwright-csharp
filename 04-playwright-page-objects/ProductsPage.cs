using Microsoft.Playwright;

namespace PlaywrightPOM
{
    public class ProductsPage
    {
        private readonly IPage _page;

        // Component composition - shared functionality handled by component
        public HeaderComponent Header { get; }

        // Page-specific locators and functionality
        public ILocator PageTitle { get; }
        public ILocator InventoryContainer { get; }
        public ILocator SortDropdown { get; }

        public ProductsPage(IPage page)
        {
            _page = page;

            // Initialize component - handles all header-related functionality
            Header = new HeaderComponent(_page);

            // Initialize page-specific elements
            PageTitle = _page.GetByText("Products");
            InventoryContainer = _page.Locator("[data-test='inventory-container']");
            SortDropdown = _page.Locator("[data-test='product_sort_container']");
        }

        // Page objects can delegate to components when appropriate
        public async Task<CartPage> NavigateToCartAsync()
        {
            return await Header.NavigateToCartAsync();
        }

        // Or provide page-specific implementations that use component data
        public async Task<ProductsPage> AddItemToCartAndVerifyAsync(string productName)
        {
            // Use a ProductCardComponent for the specific product
            var productCard = new ProductCardComponent(_page, productName);
            await productCard.AddToCartButton.ClickAsync();

            // Verify cart badge updated using header component
            var cartCount = await Header.GetShoppingCartCountAsync();
            Assert.That(int.Parse(cartCount), Is.GreaterThan(0), "Cart should contain items after adding"); // for demonstration purposes, you should leave NUnit assertions to the test layer

            return this;
        }

        // Page-specific validation methods
        public async Task ShouldBeDisplayedAsync()
        {
            await Assertions.Expect(PageTitle).ToBeVisibleAsync();
            await Assertions.Expect(InventoryContainer).ToBeVisibleAsync();
        }

        public async Task ShouldShowInventoryContainerAsync()
        {
            await Assertions.Expect(InventoryContainer).ToBeVisibleAsync();
        }
    }
}