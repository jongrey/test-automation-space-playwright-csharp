using Microsoft.Playwright;
using NUnit.Framework.Interfaces;

namespace PlaywrightFramework
{
    // BaseTest class providing shared functionality for all tests
    public abstract class BaseTest
    {
        protected IPage Page { get; private set; }
        protected IBrowserContext Context { get; private set; }
        protected IBrowser Browser { get; private set; }
        protected TestConfiguration Config { get; private set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Load configuration once per test class execution
            Config = TestConfiguration.Load();
            Config.Validate(); // Fail fast if configuration is invalid

            // Initialize browser with configuration-driven settings
            // Browser instance is shared across tests in the same class for efficiency
            Browser = await CreateBrowserAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            // Create fresh context and page for each test to ensure isolation
            // This prevents state bleeding between individual test methods
            Context = await Browser.NewContextAsync(new()
            {
                // Apply configuration-driven browser settings
                ViewportSize = Config.Browser.ViewportSize,
                // Use configuration to determine capture settings
                RecordVideoDir = Config.Capture.Videos ? Config.Capture.OutputDirectory : null,
                // Use headless mode based on configuration (local vs CI environment)
                // This allows QAs to see browser interactions locally while running headless in CI
            });

            Page = await Context.NewPageAsync();

            // Set default timeouts from configuration
            Page.SetDefaultTimeout(Config.DefaultTimeout);
            Page.SetDefaultNavigationTimeout(Config.DefaultTimeout);

            // Set up global error handling for unhandled page errors
            // This helps catch JavaScript errors that might affect test reliability
            Page.PageError += OnPageError;

            // Navigate to base URL if configured
            // This provides a consistent starting point for tests that need it
            if (!string.IsNullOrEmpty(Config.BaseUrl))
            {
                await Page.GotoAsync(Config.BaseUrl);
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            // Capture failure artifacts when tests fail
            // This provides debugging information without cluttering successful test runs
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                await CaptureFailureArtifactsAsync();
            }

            // Clean up page and context resources
            // Proper cleanup prevents resource leaks in long-running test suites
            if (Context != null)
            {
                await Context.CloseAsync();
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Close browser instance after all tests in the class complete
            if (Browser != null)
            {
                await Browser.CloseAsync();
            }
        }

        // Protected method that derived test classes can call for navigation
        // This provides consistent navigation patterns while allowing customization
        protected async Task<T> NavigateToPageAsync<T>(string url) where T : BasePage
        {
            await Page.GotoAsync(url);

            // Use reflection to create page instance with the current Page
            // This eliminates boilerplate in individual test classes
            return (T)Activator.CreateInstance(typeof(T), Page);
        }

        // Error handling for page-level JavaScript errors
        // This helps identify application issues that might not be caught by tests
        private void OnPageError(object sender, string error)
        {
            TestContext.WriteLine($"Page Error: {error}");
            // In production frameworks, you might log to external systems here
        }

        // Capture debugging information when tests fail
        // This provides crucial information for diagnosing test failures
        private async Task CaptureFailureArtifactsAsync()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var artifactPath = Path.Combine("test-results", $"{testName}_{timestamp}");

            Directory.CreateDirectory(artifactPath);

            // Capture screenshot of current page state
            await Page.ScreenshotAsync(new()
            {
                Path = Path.Combine(artifactPath, "failure-screenshot.png"),
                FullPage = true
            });

            // Save page HTML for debugging layout issues
            var html = await Page.ContentAsync();
            await File.WriteAllTextAsync(Path.Combine(artifactPath, "page-content.html"), html);

            TestContext.WriteLine($"Failure artifacts saved to: {artifactPath}");
        }

        // Factory method for creating browser with consistent configuration
        // This centralizes browser setup while allowing for environment-specific customization
        private async Task<IBrowser> CreateBrowserAsync()
        {
            var playwright = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = Config.Browser.Headless,
                SlowMo = Config.Browser.SlowMo, // Useful for debugging in development
                Timeout = Config.Browser.LaunchTimeout
            };

            // Support different browser types based on configuration
            return Config.Browser.Type.ToLower() switch
            {
                "firefox" => await playwright.Firefox.LaunchAsync(launchOptions),
                "webkit" => await playwright.Webkit.LaunchAsync(launchOptions),
                _ => await playwright.Chromium.LaunchAsync(launchOptions)
            };
        }
    }
}
