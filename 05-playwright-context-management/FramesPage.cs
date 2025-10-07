using Microsoft.Playwright;

namespace PlaywrightContextManagement
{
    public class FramesPage
    {
        private readonly IPage _page;

        public ILocator PageTitle { get; }
        public ILocator NestedFramesLink { get; }

        public FramesPage(IPage page)
        {
            _page = page;
            PageTitle = _page.Locator("h3");
            NestedFramesLink = _page.GetByRole(AriaRole.Link, new() { Name = "Nested Frames" });
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/frames");
        }

        // Navigation method that follows user workflow to nested frames
        public async Task<NestedFramesPage> NavigateToNestedFramesAsync()
        {
            var newPageTask = _page.Context.WaitForPageAsync();

            // Open Nested Frames page in a new tab using Ctrl+Click
            await NestedFramesLink.ClickAsync(new LocatorClickOptions { Modifiers = new[] { KeyboardModifier.Control }});

            // Capture the new window when it appears
            var newPage = await newPageTask;
            await newPage.WaitForLoadStateAsync();

            // Verify navigation completed successfully
            await Assertions.Expect(newPage).ToHaveURLAsync("https://the-internet.herokuapp.com/nested_frames");

            // Return Page Object representing the new window
            return new NestedFramesPage(newPage);
        }

        // Demonstrate that main page content remains accessible
        public async Task VerifyMainPageAccessibilityAsync()
        {
            // No context switching needed - main page elements remain accessible
            await Assertions.Expect(_page.GetByText("Nested Frames")).ToBeVisibleAsync();
        }
    }
}
