using Microsoft.Playwright;

namespace PlaywrightContextManagement
{
    public class JavaScriptAlertsPage
    {
        private readonly IPage _page;

        public ILocator AlertButton { get; }
        public ILocator ConfirmButton { get; }
        public ILocator PromptButton { get; }
        public ILocator Result { get; }

        public JavaScriptAlertsPage(IPage page)
        {
            _page = page;
            AlertButton = _page.GetByText("Click for JS Alert");
            ConfirmButton = _page.GetByText("Click for JS Confirm");
            PromptButton = _page.GetByText("Click for JS Prompt");
            Result = _page.Locator("#result");
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        }

        // Method that handles alert dialog as part of complete workflow
        public async Task TriggerAndHandleAlertAsync()
        {
            var alertHandled = false;
            string alertMessage = "";

            // Define the handler function separately so we can remove it later, before using another event handler
            async void AlertHandler(object sender, IDialog dialog)
            {
                alertMessage = dialog.Message;
                Assert.That(dialog.Type, Is.EqualTo(DialogType.Alert));
                await dialog.AcceptAsync();
                alertHandled = true;
            }

            // Attach the handler
            _page.Dialog += AlertHandler;

            try
            {
                // Trigger the dialog
                await AlertButton.ClickAsync();

                // Verify dialog was handled correctly
                Assert.That(alertHandled, Is.True, "Alert dialog should have appeared");
                await Assertions.Expect(Result).ToHaveTextAsync("You successfully clicked an alert");

                TestContext.WriteLine($"Handled alert with message: {alertMessage}");
            }
            finally
            {
                // Always remove the handler, even if an exception occurs
                _page.Dialog -= AlertHandler;
            }
        }

        // Method demonstrating confirm dialog with decision logic
        public async Task HandleConfirmDialogAsync(bool shouldAccept)
        {
            var confirmHandled = false;

            async void ConfirmHandler(object sender, IDialog dialog)
            {
                Assert.That(dialog.Type, Is.EqualTo(DialogType.Confirm));

                if (shouldAccept)
                {
                    await dialog.AcceptAsync();
                }
                else
                {
                    await dialog.DismissAsync();
                }
                confirmHandled = true;
            }

            _page.Dialog += ConfirmHandler;

            try
            {
                await ConfirmButton.ClickAsync();

                Assert.That(confirmHandled, Is.True, "Confirm dialog should have been handled");

                var expectedResult = shouldAccept ? "You clicked: Ok" : "You clicked: Cancel";
                await Assertions.Expect(Result).ToHaveTextAsync(expectedResult);
            }
            finally
            {
                _page.Dialog -= ConfirmHandler;
            }
        }

        // Method showing prompt dialog with input validation
        public async Task HandlePromptDialogAsync(string inputText)
        {
            var promptHandled = false;

            async void PromptHandler(object sender, IDialog dialog)
            {
                Assert.That(dialog.Type, Is.EqualTo(DialogType.Prompt));
                await dialog.AcceptAsync(inputText);
                promptHandled = true;
            }

            _page.Dialog += PromptHandler;

            try
            {
                await PromptButton.ClickAsync();

                Assert.That(promptHandled, Is.True, "Prompt dialog should have been handled");
                await Assertions.Expect(Result).ToHaveTextAsync($"You entered: {inputText}");
            }
            finally
            {
                _page.Dialog -= PromptHandler;
            }
        }
    }
}
