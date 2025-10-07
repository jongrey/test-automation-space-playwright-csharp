using Microsoft.Playwright;

public class ProductCardComponent
{
    private readonly ILocator _container;

    // All these locators are automatically scoped to the container
    public ILocator ProductTitle { get; }
    public ILocator ProductPrice { get; }
    public ILocator AddToCartButton { get; }
    public ILocator ProductImage { get; }

    public ProductCardComponent(IPage page, string productName)
    {
        // Find the specific product card container
        _container = page.Locator(".inventory_item")
            .Filter(new() { HasText = productName });

        // These locators will only search within this specific product card
        ProductTitle = _container.Locator(".inventory_item_name");
        ProductPrice = _container.Locator(".inventory_item_price");
        AddToCartButton = _container.GetByRole(AriaRole.Button);
        ProductImage = _container.GetByRole(AriaRole.Img);
    }
}