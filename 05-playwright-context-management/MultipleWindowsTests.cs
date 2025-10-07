using Microsoft.Playwright.NUnit;

namespace PlaywrightContextManagement
{
    public class MultipleWindowsTests : PageTest
    {
        [Test]
        public async Task DemonstrateMultiWindowWorkflow()
        {
            // Navigate to the multiple windows test page
            var windowsPage = new MultipleWindowsPage(Page);
            await windowsPage.NavigateAsync();

            // Verify we start on the correct page
            await windowsPage.VerifyOriginalPageTitleAsync();

            // Open new window using event-driven pattern
            var newWindow = await windowsPage.OpenNewWindowAsync();

            // Verify the new window opened with correct content
            await newWindow.VerifyNewWindowContentAsync();
            var newWindowTitle = await newWindow.GetWindowTitleAsync();
            TestContext.WriteLine($"New window title: {newWindowTitle}");

            // Verify original window remains accessible without switching
            await windowsPage.VerifyOriginalPageTitleAsync();
            TestContext.WriteLine("Original window remains accessible");

            // Compare content between windows
            var originalTitle = await Page.TitleAsync();
            Assert.That(originalTitle, Is.Not.EqualTo(newWindowTitle),
                "Windows should have different titles");

            // Perform actions in new window
            await newWindow.VerifyNewWindowContentAsync();

            // Return to original window for additional testing
            // (No explicit switching needed - original page object still works)
            await windowsPage.VerifyOriginalPageTitleAsync();

            // Clean up the new window
            await newWindow.CloseAsync();

            // Verify original window still functions after cleanup
            await windowsPage.VerifyOriginalPageTitleAsync();
            TestContext.WriteLine("Multi-window workflow completed successfully");
        }
    }
}
