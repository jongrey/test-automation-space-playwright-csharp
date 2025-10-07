using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace PlaywrightPOM
{
    public class CheckoutPage
    {
        private readonly IPage _page;
        public ILocator FirstNameInput { get; }
        public ILocator LastNameInput { get; }
        public ILocator PostalCodeInput { get; }
        public ILocator ContinueButton { get; }

        public CheckoutPage(IPage page)
        {
            _page = page;
            FirstNameInput = _page.Locator("[data-test='firstName']");
            LastNameInput = _page.Locator("[data-test='lastName']");
            PostalCodeInput = _page.Locator("[data-test='postalCode']");
            ContinueButton = _page.Locator("[data-test='continue']");
        }

        public async Task FillShippingInformationAsync(string firstName, string lastName, string postalCode)
        {
            await FirstNameInput.FillAsync(firstName);
            await LastNameInput.FillAsync(lastName);
            await PostalCodeInput.FillAsync(postalCode);
        }

        public async Task<CheckoutOverviewPage> ContinueToOverviewAsync()
        {
            await ContinueButton.ClickAsync();
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*checkout-step-two.html"));
            return new CheckoutOverviewPage(_page);
        }
    }
}
