using Microsoft.Playwright.NUnit;

namespace PlaywrightContextManagement
{
    public class AlertsTests : PageTest
    {
        [Test]
        public async Task DemonstrateDialogWorkflowIntegration()
        {
            var alertsPage = new JavaScriptAlertsPage(Page);
            await alertsPage.NavigateAsync();

            // Test alert handling in sequence
            await alertsPage.TriggerAndHandleAlertAsync();

            // Test confirm dialog with acceptance
            await alertsPage.HandleConfirmDialogAsync(shouldAccept: true);

            // Test confirm dialog with dismissal
            await alertsPage.HandleConfirmDialogAsync(shouldAccept: false);

            // Test prompt dialog with custom input
            await alertsPage.HandlePromptDialogAsync("Playwright automation test");

            TestContext.WriteLine("All dialog types handled successfully within integrated workflow");
        }
    }
}
