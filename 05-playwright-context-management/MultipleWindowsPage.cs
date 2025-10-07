using Microsoft.Playwright;

namespace PlaywrightContextManagement;
public class MultipleWindowsPage
{
    private readonly IPage _page;

    public ILocator ClickHereLink { get; }
    public ILocator PageTitle { get; }

    public MultipleWindowsPage(IPage page)
    {
        _page = page;
        ClickHereLink = _page.GetByText("Click Here");
        PageTitle = _page.Locator("h3");
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/windows");
    }

    public async Task<NewWindowPage> OpenNewWindowAsync()
    {
        // Set up listener before triggering the action that opens new window
        var newPageTask = _page.Context.WaitForPageAsync();

        // Click the link that opens a new window
        await ClickHereLink.ClickAsync();

        // Capture the new window when it appears
        var newPage = await newPageTask;
        await newPage.WaitForLoadStateAsync();

        // Return Page Object representing the new window
        return new NewWindowPage(newPage);
    }

    public async Task VerifyOriginalPageTitleAsync()
    {
        // Verify original page remains accessible without window switching
        await Assertions.Expect(PageTitle).ToHaveTextAsync("Opening a new window");
    }
}