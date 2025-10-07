using Microsoft.Playwright;

namespace PlaywrightAdvancedInteractions
{
    public class HoversPage
    {
        private readonly IPage _page;

        public ILocator PageTitle => _page.Locator("h3");
        public ILocator UserAvatars => _page.Locator(".figure");

        public HoversPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/hovers");
        }

        // Method demonstrating hover interaction with hidden content
        public async Task HoverOverUserAsync(int userIndex)
        {
            var userAvatar = UserAvatars.Nth(userIndex);

            // Hover over user avatar to reveal hidden content
            await userAvatar.HoverAsync();

            // Verify hidden content becomes visible
            var userDetails = userAvatar.Locator(".figcaption");
            await Assertions.Expect(userDetails).ToBeVisibleAsync();

            // Verify specific user information appears
            var userName = userDetails.Locator("h5");
            var userLink = userDetails.GetByRole(AriaRole.Link, new() { Name = "View profile" });

            await Assertions.Expect(userName).ToBeVisibleAsync();
            await Assertions.Expect(userLink).ToBeVisibleAsync();

            var nameText = await userName.TextContentAsync();
            TestContext.WriteLine($"Hovered over {nameText}");
        }
    }
}
