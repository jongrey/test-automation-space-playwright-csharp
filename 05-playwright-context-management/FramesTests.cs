using Microsoft.Playwright.NUnit;

namespace PlaywrightContextManagement
{
    public class FramesTests : PageTest
    {
        [Test]
        public async Task DemonstrateNestedFrameInteraction()
        {
            // Start from the main frames page
            var framesPage = new FramesPage(Page);
            await framesPage.NavigateAsync();

            // Navigate to nested frames following the user journey
            var nestedFramesPage = await framesPage.NavigateToNestedFramesAsync();

            // Verify we can interact with all nested frame content
            await nestedFramesPage.VerifyFrameContentAsync();

            // Extract content for detailed verification
            var frameContents = await nestedFramesPage.GetAllFrameContentsAsync();

            // Verify each frame contains expected content
            Assert.That(frameContents["left"].Trim(), Is.EqualTo("LEFT"));
            Assert.That(frameContents["middle"].Trim(), Is.EqualTo("MIDDLE"));
            Assert.That(frameContents["right"].Trim(), Is.EqualTo("RIGHT"));
            Assert.That(frameContents["bottom"].Trim(), Is.EqualTo("BOTTOM"));

            // Verify main page remains accessible throughout iframe interactions
            await framesPage.VerifyMainPageAccessibilityAsync();

            TestContext.WriteLine("Successfully completed frame workflow from navigation through interaction");
        }
    }
}
