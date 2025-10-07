using Microsoft.Playwright;

namespace PlaywrightAdvancedInteractions
{
    public class DropdownPage
    {
        private readonly IPage _page;

        public ILocator Dropdown => _page.Locator("#dropdown");

        public DropdownPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/dropdown");
        }

        // Method demonstrating different dropdown selection approaches
        public async Task SelectDropdownOptionsAsync()
        {
            // Select by visible text
            await Dropdown.SelectOptionAsync(new SelectOptionValue { Label = "Option 1" });
            await Assertions.Expect(Dropdown).ToHaveValueAsync("1");

            // Select by value attribute
            await Dropdown.SelectOptionAsync(new SelectOptionValue { Value = "2" });
            await Assertions.Expect(Dropdown).ToHaveValueAsync("2");

            // Select by index
            await Dropdown.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            await Assertions.Expect(Dropdown).ToHaveValueAsync("1");
        }
    }
}
