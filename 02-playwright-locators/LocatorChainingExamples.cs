using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightLocators.Tests;

public class LocatorChainingExamples : PageTest
{
    [SetUp]
    public async Task LoginAndNavigateToProducts()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.GetByPlaceholder("Username").FillAsync("standard_user");
        await Page.GetByPlaceholder("Password").FillAsync("secret_sauce");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task ContainerBasedScoping_EnablesPreciseTargeting()
    {
        // Find specific product card using text content
        var backpackCard = Page.GetByText("Sauce Labs Backpack")
            .Locator("xpath=ancestor::div[contains(@class, 'inventory_item')]");

        // Chain locators within the product card for precision
        var backpackPrice = backpackCard.GetByText(new Regex(@"\$\d+\.\d{2}"));
        var backpackDescription = backpackCard.GetByText("carry.AllTheThings()");
        var backpackAddButton = backpackCard.GetByText("Add to cart");

        // Verify all elements are found within the correct product
        await Expect(backpackPrice).ToBeVisibleAsync();
        await Expect(backpackDescription).ToBeVisibleAsync();
        await Expect(backpackAddButton).ToBeVisibleAsync();

        // Interact with the specific product
        await backpackAddButton.ClickAsync();

        // Verify the button text changed within this specific product
        await Expect(backpackCard.GetByText("Remove")).ToBeVisibleAsync();
    }

    [Test]
    public async Task FilterBasedRefinement_HandlesComplexScenarios()
    {
        // Find products within specific price ranges using filtering
        var expensiveProducts = Page.Locator(".inventory_item")
            .Filter(new() { HasTextRegex = new Regex(@"\$[2-9]\d\.\d{2}") }); // $20+ items

        var affordableProducts = Page.Locator(".inventory_item")
            .Filter(new() { HasTextRegex = new Regex(@"\$(?:0?[0-9]|1[0-9])\.\d{2}") }); // Under $20

        // Count products in each category
        var expensiveCount = await expensiveProducts.CountAsync();
        var affordableCount = await affordableProducts.CountAsync();

        TestContext.WriteLine($"Expensive products: {expensiveCount}");
        TestContext.WriteLine($"Affordable products: {affordableCount}");

        Assert.That(expensiveCount + affordableCount, Is.EqualTo(6),
            "Should categorize all 6 products");
    }

    [Test]
    public async Task PositionalNavigation_HandlesSiblingRelationships()
    {
        // Find product name and navigate to its price (sibling element)
        var productName = Page.GetByText("Sauce Labs Bolt T-Shirt");
        var productContainer = productName.Locator("xpath=ancestor::div[contains(@class, 'inventory_item')]");

        // Navigate to price within the same container
        var productPrice = productContainer.GetByText(new Regex(@"\$\d+\.\d{2}"));

        // Navigate to image within the same container
        var productImage = productContainer.GetByRole(AriaRole.Img);

        await Expect(productPrice).ToBeVisibleAsync();
        await Expect(productImage).ToBeVisibleAsync();

        // Verify the image has appropriate alt text
        await Expect(productImage).ToHaveAttributeAsync("alt", new Regex(".*Sauce Labs Bolt T-Shirt.*"));
    }
}