using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace PlaywrightFramework
{
    // Configuration classes that provide type safety and clear structure
    public class TestConfiguration
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int DefaultTimeout { get; set; } = 30000;
        public BrowserConfiguration Browser { get; set; } = new();
        public TestDataConfiguration TestData { get; set; } = new();
        public CaptureConfiguration Capture { get; set; } = new();

        // Static factory method for loading configuration from appsettings.json
        // This centralizes configuration loading while handling environment-specific overrides
        public static TestConfiguration Load()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() // Allow environment variables to override any setting
                .Build();

                var testConfig = new TestConfiguration();
                configuration.GetSection("TestConfiguration").Bind(testConfig);

                return testConfig;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load configuration. Check files and environment variables.", ex);
            }
        }

        // Validation method to ensure configuration is complete and valid
        // This catches configuration errors early rather than during test execution
        public void Validate()
        {
            if (string.IsNullOrEmpty(BaseUrl))
                throw new InvalidOperationException("BaseUrl must be configured");

            if (DefaultTimeout <= 0)
                throw new InvalidOperationException("DefaultTimeout must be positive");

            Browser.Validate();
            TestData.Validate();
        }
    }

    // Browser-specific configuration with sensible defaults and validation
    public class BrowserConfiguration
    {
        public string Type { get; set; } = "chromium";
        public bool Headless { get; set; } = false;
        public int SlowMo { get; set; } = 0;
        public int ViewportWidth { get; set; } = 1920;
        public int ViewportHeight { get; set; } = 1080;
        public int LaunchTimeout { get; set; } = 30000;

        // Computed property that provides Playwright viewport configuration
        // This demonstrates how configuration classes can provide derived values
        public ViewportSize ViewportSize => new() { Width = ViewportWidth, Height = ViewportHeight };

        public void Validate()
        {
            var validBrowserTypes = new[] { "chromium", "firefox", "webkit" };
            if (!validBrowserTypes.Contains(Type.ToLower()))
                throw new InvalidOperationException($"Browser type must be one of: {string.Join(", ", validBrowserTypes)}");

            if (ViewportWidth <= 0 || ViewportHeight <= 0)
                throw new InvalidOperationException("Viewport dimensions must be positive");
        }
    }

    // Test data configuration that supports multiple user types and scenarios
    public class TestDataConfiguration
    {
        public UserCredentials DefaultUser { get; set; } = new();
        public UserCredentials AdminUser { get; set; } = new();
        public UserCredentials ProblemUser { get; set; } = new();

        public void Validate()
        {
            DefaultUser.Validate("DefaultUser");
            // AdminUser and ProblemUser validation can be optional depending on test needs
        }
    }

    // User credentials with validation to ensure completeness
    public class UserCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public void Validate(string userType)
        {
            if (string.IsNullOrEmpty(Username))
                throw new InvalidOperationException($"{userType}.Username must be configured");
            if (string.IsNullOrEmpty(Password))
                throw new InvalidOperationException($"{userType}.Password must be configured");
        }
    }

    // Capture configuration for test artifacts and debugging information
    public class CaptureConfiguration
    {
        public bool Screenshots { get; set; } = true;
        public bool Videos { get; set; } = false;
        public bool Traces { get; set; } = true;
        public string OutputDirectory { get; set; } = "test-results";
    }
}
