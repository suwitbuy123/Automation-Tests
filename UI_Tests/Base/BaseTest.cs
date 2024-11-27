using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UI_Tests.Base
{
    public class BaseTest
    {
        protected IWebDriver driver = null!; // WebDriver instance for browser interactions

        [SetUp]
        public void SetUp()
        {
            // Initializes the WebDriver, maximizes the browser, and navigates to the base URL
            var options = new ChromeOptions();
            options.AddArguments("--headless"); // Run in headless mode
            options.AddArguments("--no-sandbox"); // Bypass OS security model
            options.AddArguments("--disable-dev-shm-usage"); // Overcome limited resource problems
            options.AddArguments("--disable-gpu"); // Disable GPU
            options.AddArguments("--window-size=1920,1080"); // Set window size
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.saucedemo.com");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                // Captures a screenshot if a test fails for better debugging and reporting
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    CaptureScreenshot();
                }
            }
            finally
            {
                // Closes the browser after each test to release resources
                driver?.Quit();
            }
        }

        /// <summary>
        /// Waits for an element to appear on the page within the specified timeout.
        /// </summary>
        /// <param name="by">Selector for the target element</param>
        /// <param name="timeoutInSeconds">Maximum wait time in seconds</param>
        /// <returns>True if the element appears, false otherwise</returns>
        protected bool WaitForElement(By by, int timeoutInSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(d => d.FindElement(by).Displayed);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"Element with selector '{by}' was not found within {timeoutInSeconds} seconds.");
                return false;
            }
        }

        /// <summary>
        /// Captures a screenshot if a test encounters an error.
        /// </summary>
        private void CaptureScreenshot()
        {
            if (driver == null)
            {
                TestContext.WriteLine("Driver is not initialized. Screenshot capture skipped.");
                return;
            }

            try
            {
                string screenshotsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                if (!Directory.Exists(screenshotsFolder))
                {
                    Directory.CreateDirectory(screenshotsFolder);
                }

                string fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                string filePath = Path.Combine(screenshotsFolder, fileName);

                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(filePath);

                TestContext.WriteLine($"Screenshot captured: {filePath}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
        }


        /// <summary>
        /// Waits for the current URL to match the expected URL within the specified timeout.
        /// </summary>
        /// <param name="expectedUrl">The expected URL to validate</param>
        /// <param name="timeoutInSeconds">Maximum wait time in seconds</param>
        /// <returns>True if the URL matches, false otherwise</returns>
        protected bool WaitForUrl(string expectedUrl, int timeoutInSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(d => d.Url.Equals(expectedUrl, StringComparison.OrdinalIgnoreCase));
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"Expected URL '{expectedUrl}' was not reached within {timeoutInSeconds} seconds. Current URL: {driver.Url}");
                return false;
            }
        }
    }
}
