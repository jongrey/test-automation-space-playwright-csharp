using Microsoft.Playwright;

namespace PlaywrightContextManagement
{
    public class NestedFramesPage
    {
        private readonly IPage _page;
        private readonly IFrameLocator _topFrame;
        private readonly IFrameLocator _leftFrame;
        private readonly IFrameLocator _middleFrame;
        private readonly IFrameLocator _rightFrame;
        private readonly IFrameLocator _bottomFrame;

        public NestedFramesPage(IPage page)
        {
            _page = page;

            // Create frame locators for the nested frame structure
            _topFrame = _page.FrameLocator("frame[name='frame-top']");

            // Left and middle frames are nested within the top frame
            _leftFrame = _topFrame.FrameLocator("frame[name='frame-left']");
            _middleFrame = _topFrame.FrameLocator("frame[name='frame-middle']");
            _rightFrame = _topFrame.FrameLocator("frame[name='frame-right']");

            // Bottom frame is at the same level as top frame
            _bottomFrame = _page.FrameLocator("frame[name='frame-bottom']");
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/nested_frames");
        }

        // Method demonstrating interaction with nested frame content
        public async Task VerifyFrameContentAsync()
        {
            // Interact with content in the left frame (nested within top frame)
            await Assertions.Expect(_leftFrame.GetByText("LEFT")).ToBeVisibleAsync();

            // Interact with content in the middle frame (also nested within top frame)
            await Assertions.Expect(_middleFrame.GetByText("MIDDLE")).ToBeVisibleAsync();

            // Interact with content in the right frame
            await Assertions.Expect(_rightFrame.GetByText("RIGHT")).ToBeVisibleAsync();

            // Interact with content in the bottom frame (separate from top frame hierarchy)
            await Assertions.Expect(_bottomFrame.GetByText("BOTTOM")).ToBeVisibleAsync();
        }

        // Method showing how to extract content from frames for test verification
        public async Task<Dictionary<string, string>> GetAllFrameContentsAsync()
        {
            var frameContents = new Dictionary<string, string>();

            // Extract text content from each frame for verification
            frameContents["left"] = await _leftFrame.Locator("body").TextContentAsync() ?? "";
            frameContents["middle"] = await _middleFrame.Locator("body").TextContentAsync() ?? "";
            frameContents["right"] = await _rightFrame.Locator("body").TextContentAsync() ?? "";
            frameContents["bottom"] = await _bottomFrame.Locator("body").TextContentAsync() ?? "";

            return frameContents;
        }
    }
}
