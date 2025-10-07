using Microsoft.Playwright;

namespace PlaywrightAdvancedInteractions
{
    public class CheckboxRadioPage
    {
        private readonly IPage _page;

        public CheckboxRadioPage(IPage page)
        {
            _page = page;
        }

        // Method demonstrating checkbox interactions
        public async Task TestCheckboxInteractionsAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/checkboxes");

            var checkboxes = _page.Locator("input[type='checkbox']");
            var firstCheckbox = checkboxes.First;

            await Assertions.Expect(firstCheckbox).Not.ToBeCheckedAsync();

            await firstCheckbox.CheckAsync();
            await Assertions.Expect(firstCheckbox).ToBeCheckedAsync();

            await firstCheckbox.UncheckAsync();
            await Assertions.Expect(firstCheckbox).Not.ToBeCheckedAsync();
        }
    }
}
