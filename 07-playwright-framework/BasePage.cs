using Microsoft.Playwright;

namespace PlaywrightFramework
{
    // BasePage class providing shared functionality for all Page Objects
    public abstract class BasePage
    {
        protected readonly IPage _page;
        protected readonly TestConfiguration _config;

        // Common locators that appear on most pages
        // These represent truly shared UI elements like navigation and notifications
        public ILocator LoadingSpinner => _page.Locator(".loading-spinner, .spinner");
        public ILocator ErrorMessage => _page.Locator(".error-message, .alert-error");
        public ILocator SuccessMessage => _page.Locator(".success-message, .alert-success");

        protected BasePage(IPage page)
        {
            _page = page;
            _config = TestConfiguration.Load();
        }

        // Navigation utilities that provide consistent behavior across all pages
        // These methods handle common navigation patterns while allowing for page-specific customization
        protected async Task WaitForPageLoadAsync()
        {
            // Wait for network activity to settle
            // This is often more reliable than waiting for specific elements
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Wait for any loading indicators to disappear
            // This handles dynamic content loading that occurs after initial page load
            if (await LoadingSpinner.IsVisibleAsync())
            {
                await Assertions.Expect(LoadingSpinner).Not.ToBeVisibleAsync();
            }
        }

        // Error handling utilities that provide consistent error detection
        // These methods help identify application-level errors that might affect test validity
        protected async Task<bool> HasErrorMessageAsync()
        {
            return await ErrorMessage.IsVisibleAsync();
        }

        protected async Task<string> GetErrorMessageTextAsync()
        {
            if (await HasErrorMessageAsync())
            {
                return await ErrorMessage.TextContentAsync() ?? "";
            }
            return string.Empty;
        }

        // Success message handling for positive test scenarios
        protected async Task<bool> HasSuccessMessageAsync()
        {
            return await SuccessMessage.IsVisibleAsync();
        }

        // Utility method for safe text extraction that handles missing elements gracefully
        // This prevents NullReferenceExceptions when elements are not present
        protected async Task<string> GetTextSafelyAsync(ILocator locator)
        {
            try
            {
                if (await locator.IsVisibleAsync())
                {
                    return await locator.TextContentAsync() ?? "";
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Warning: Could not get text from locator: {ex.Message}");
            }
            return string.Empty;
        }

        // Consistent waiting strategy for dynamic content
        // This provides a standard approach to handling timing issues across all pages
        protected async Task WaitForElementToBeReadyAsync(ILocator element, int timeoutMs = 5000)
        {
            await Assertions.Expect(element).ToBeVisibleAsync(new() { Timeout = timeoutMs });
            await Assertions.Expect(element).ToBeEnabledAsync(new() { Timeout = timeoutMs });
        }

        // URL validation utilities that help verify navigation success
        // These methods provide consistent patterns for checking page identity
        protected async Task<bool> IsCurrentUrlAsync(string expectedUrl)
        {
            var currentUrl = _page.Url;
            return currentUrl.Equals(expectedUrl, StringComparison.OrdinalIgnoreCase);
        }

        protected async Task<bool> UrlContainsAsync(string urlFragment)
        {
            var currentUrl = _page.Url;
            return currentUrl.Contains(urlFragment, StringComparison.OrdinalIgnoreCase);
        }

        // Screenshot utility for debugging and documentation
        // This provides consistent screenshot functionality across all Page Objects
        protected async Task<string> TakeScreenshotAsync(string filename = null)
        {
            filename ??= $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            var screenshotPath = Path.Combine("screenshots", filename);

            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));
            await _page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });

            return screenshotPath;
        }
    }
}
