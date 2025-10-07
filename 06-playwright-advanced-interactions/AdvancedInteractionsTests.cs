using Microsoft.Playwright.NUnit;

namespace PlaywrightAdvancedInteractions
{
    public class AdvancedInteractionsTests : PageTest
    {
        [Test]
        public async Task DemonstrateAdvancedMouseWorkflows()
        {
            // Test hover interactions
            var hoversPage = new HoversPage(Page);
            await hoversPage.NavigateAsync();
            await hoversPage.HoverOverUserAsync(0); // Hover over first user

            // Test drag-and-drop interactions
            var dragDropPage = new DragAndDropPage(Page);
            await dragDropPage.NavigateAsync();
            await dragDropPage.SwapElementsAsync();
            await dragDropPage.VerifyDragDropBehaviorAsync();

            TestContext.WriteLine("Advanced mouse interaction workflow completed successfully");
        }
    }
}
