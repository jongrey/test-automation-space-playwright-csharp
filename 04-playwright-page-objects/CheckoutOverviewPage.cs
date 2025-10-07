using Microsoft.Playwright;

namespace PlaywrightPOM
{
    // This class can be expanded with locators and methods specific to the checkout overview page
    public class CheckoutOverviewPage
    {
        private readonly IPage _page;

        public CheckoutOverviewPage(IPage page)
        {
            _page = page;
        }
    }
}
