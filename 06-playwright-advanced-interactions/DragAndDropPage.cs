using Microsoft.Playwright;

namespace PlaywrightAdvancedInteractions
{
    public class DragAndDropPage
    {
        private readonly IPage _page;

        public ILocator ColumnA => _page.Locator("#column-a");
        public ILocator ColumnB => _page.Locator("#column-b");

        public DragAndDropPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/drag_and_drop");
        }

        // Simple drag-and-drop using built-in method
        public async Task SwapElementsAsync()
        {
            // Verify initial state
            await Assertions.Expect(ColumnA).ToHaveTextAsync("A");
            await Assertions.Expect(ColumnB).ToHaveTextAsync("B");

            // Perform drag-and-drop operation
            await ColumnA.DragToAsync(ColumnB);

            // Verify elements swapped
            await Assertions.Expect(ColumnA).ToHaveTextAsync("B");
            await Assertions.Expect(ColumnB).ToHaveTextAsync("A");

            TestContext.WriteLine("Successfully swapped elements using drag-and-drop");
        }

        // Method demonstrating drag-and-drop verification
        public async Task VerifyDragDropBehaviorAsync()
        {
            var initialAText = await ColumnA.TextContentAsync();
            var initialBText = await ColumnB.TextContentAsync();

            // Perform drag operation
            await ColumnA.DragToAsync(ColumnB);

            // Verify content actually changed
            var finalAText = await ColumnA.TextContentAsync();
            var finalBText = await ColumnB.TextContentAsync();

            Assert.That(finalAText, Is.EqualTo(initialBText));
            Assert.That(finalBText, Is.EqualTo(initialAText));

            TestContext.WriteLine($"Drag-and-drop verified: {initialAText} and {initialBText} swapped positions");
        }
    }
}
