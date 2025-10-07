using Microsoft.Playwright;

namespace PlaywrightAdvancedInteractions
{
    public class FileUploadPage
    {
        private readonly IPage _page;

        public ILocator FileInput => _page.Locator("#file-upload");
        public ILocator UploadButton => _page.Locator("#file-submit");
        public ILocator UploadedFiles => _page.Locator("#uploaded-files");

        public FileUploadPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/upload");
        }

        // Method demonstrating file upload with actual files
        public async Task UploadFileAsync()
        {
            // Create temporary test file
            var tempFilePath = Path.GetTempFileName();
            var testContent = "Test file content for upload verification";
            await File.WriteAllTextAsync(tempFilePath, testContent);

            try
            {
                // Set file on input element
                await FileInput.SetInputFilesAsync(tempFilePath);

                // Submit the upload
                await UploadButton.ClickAsync();

                // Verify successful upload
                await Assertions.Expect(_page.GetByText("File Uploaded!")).ToBeVisibleAsync();
                await Assertions.Expect(UploadedFiles).ToContainTextAsync(Path.GetFileName(tempFilePath));

                TestContext.WriteLine($"Successfully uploaded file: {Path.GetFileName(tempFilePath)}");
            }
            finally
            {
                // Clean up temporary file
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        // Method demonstrating programmatic file creation
        public async Task UploadProgrammaticFileAsync()
        {
            // Create file content programmatically without filesystem
            var fileContent = "Programmatically created test content";
            var fileName = "test-upload.txt";

            await FileInput.SetInputFilesAsync(new FilePayload
            {
                Name = fileName,
                MimeType = "text/plain",
                Buffer = System.Text.Encoding.UTF8.GetBytes(fileContent)
            });

            await UploadButton.ClickAsync();

            // Verify upload success
            await Assertions.Expect(_page.GetByText("File Uploaded!")).ToBeVisibleAsync();
            await Assertions.Expect(UploadedFiles).ToContainTextAsync(fileName);

            TestContext.WriteLine($"Successfully uploaded programmatic file: {fileName}");
        }
    }
}
