using Microsoft.Playwright;

namespace PlaywrightContextManagement
{
    public class NewWindowPage
    {
        private readonly IPage _page;

        public ILocator PageTitle { get; }
        public ILocator PageContent { get; }

        public NewWindowPage(IPage page)
        {
            _page = page;
            PageTitle = _page.Locator("h3");
            PageContent = _page.Locator("body");
        }

        public async Task VerifyNewWindowContentAsync()
        {
            await Assertions.Expect(PageTitle).ToHaveTextAsync("New Window");
            await Assertions.Expect(PageContent).ToContainTextAsync("New Window");
        }

        public async Task CloseAsync()
        {
            await _page.CloseAsync();
        }

        public async Task<string> GetWindowTitleAsync()
        {
            return await _page.TitleAsync();
        }
    }
}
