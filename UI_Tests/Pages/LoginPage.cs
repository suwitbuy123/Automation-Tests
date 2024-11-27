using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace UI_Tests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;

        // Constants for report storage
        private const string ReportFolder = "Reports";
        private const string TestResultsFile = "TestResults.txt";

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver; // Initializes the WebDriver instance
        }

        /// <summary>
        /// Retrieves error messages displayed on the login page, if any.
        /// Logs the error to a report file for further debugging.
        /// </summary>
        /// <returns>The error message text, or a default message if no error is found.</returns>
        public string GetErrorMessage()
        {
            try
            {
                // Locate the error message element
                var errorMessageElement = driver.FindElement(By.CssSelector(".error-message-container.error"));
                string errorMessage = errorMessageElement.Text;

                // Log the error message to the report
                WriteToReport($"Validation Error: {errorMessage}");
                return errorMessage;
            }
            catch (NoSuchElementException)
            {
                // Return a default message if no error element is found
                return "No error message displayed.";
            }
        }

        /// <summary>
        /// Writes a log entry to the test results report file.
        /// Ensures the report directory exists before writing.
        /// </summary>
        /// <param name="message">The message to log in the report file.</param>
        private void WriteToReport(string message)
        {
            string reportFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ReportFolder);
            string reportFilePath = Path.Combine(reportFolderPath, TestResultsFile);

            // Ensure the Reports folder exists
            if (!Directory.Exists(reportFolderPath))
            {
                Directory.CreateDirectory(reportFolderPath);
            }

            // Append the message to the report file
            using (StreamWriter writer = new StreamWriter(reportFilePath, append: true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
            }
        }

        /// <summary>
        /// Checks whether login was successful by verifying the presence of an expected element.
        /// </summary>
        /// <returns>True if the login was successful, false otherwise.</returns>
        public bool IsLoginSuccessful()
        {
            try
            {
                // Validate that the inventory container is displayed post-login
                var productPageElement = driver.FindElement(By.Id("inventory_container"));
                return productPageElement.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false; // Login is unsuccessful if the element is not found
            }
        }

        /// <summary>
        /// Performs a logout operation by interacting with the menu and logout link.
        /// Logs the outcome of the logout attempt.
        /// </summary>
        public void Logout()
        {
            try
            {
                TestContext.WriteLine("Attempting to log out...");

                // Wait for and click the menu button
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var menuButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("react-burger-menu-btn")));
                menuButton.Click();

                // Wait for and click the logout link
                var logoutLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("logout_sidebar_link")));
                logoutLink.Click();

                TestContext.WriteLine("Logout successful.");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Logout failed: {ex.Message}");
                throw; // Re-throw the exception to fail the test
            }
        }

        /// <summary>
        /// Performs a login operation by entering credentials and clicking the login button.
        /// Checks for success or logs any error messages if login fails.
        /// </summary>
        /// <param name="username">The username to use for login.</param>
        /// <param name="password">The password to use for login.</param>
        /// <returns>True if login is successful, false otherwise.</returns>
        public bool Login(string username, string password)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Enter the username
            var usernameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("user-name")));
            usernameField.Clear();
            usernameField.SendKeys(username);

            // Enter the password
            var passwordField = driver.FindElement(By.Id("password"));
            passwordField.Clear();
            passwordField.SendKeys(password);

            // Click the login button
            var loginButton = driver.FindElement(By.Id("login-button"));
            loginButton.Click();

            // Check for error messages
            try
            {
                var errorMessageElement = driver.FindElement(By.CssSelector(".error-message-container.error"));
                string errorMessage = errorMessageElement.Text;

                // Log the error
                WriteToReport($"Login failed for user '{username}': {errorMessage}");
                TestContext.WriteLine($"Login failed for user '{username}': {errorMessage}");

                return false; // Login was not successful
            }
            catch (NoSuchElementException)
            {
                // No error message detected, login is successful
                return true;
            }
        }
    }
}
