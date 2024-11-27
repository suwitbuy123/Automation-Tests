using Newtonsoft.Json;
using NUnit.Framework;
using UI_Tests.Base;
using UI_Tests.Pages;

namespace UI_Tests.Tests
{
    public class LoginTests : BaseTest
    {
        private const string ReportFolder = "Reports";
        private const string TestResultsFile = "TestResults.txt";

        [Test]
        public void LoginWithValidCredentials()
        {
            try
            {
                // Define the path to the JSON file containing test credentials
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/TestCredentials.json");

                // Deserialize the JSON data into a TestCredentials object
                var credentials = JsonConvert.DeserializeObject<TestCredentials>(File.ReadAllText(jsonFilePath));

                // Validate that the required validUser data is present in the JSON file
                if (credentials?.ValidUser == null ||
                    string.IsNullOrEmpty(credentials.ValidUser.Username) ||
                    string.IsNullOrEmpty(credentials.ValidUser.Password))
                {
                    WriteToReport("Test data is missing or invalid for validUser.");
                    throw new Exception("Test data is missing or invalid for validUser.");
                }

                // Extract username and password from the JSON object
                string username = credentials.ValidUser.Username;
                string password = credentials.ValidUser.Password;

                // Arrange: Initialize the LoginPage object for interaction
                var loginPage = new LoginPage(driver);

                // Act: Attempt to log in with the provided credentials
                loginPage.Login(username, password);

                // Assert: Verify the login result by checking the resulting URL
                if (driver.Url != "https://www.saucedemo.com/inventory.html")
                {
                    // If login fails, retrieve and log the error message
                    string errorMessage = loginPage.GetErrorMessage();
                    string logMessage = $"Login failed for user: {username}. Error: {errorMessage}";
                    WriteToReport(logMessage);
                    Assert.Fail(logMessage);
                }
                else
                {
                    // Log a success message if the login is successful
                    string successMessage = $"Login successful for user: {username}";
                    WriteToReport(successMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions and log the details
                WriteToReport($"Unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Writes test results to a log file for reporting purposes.
        /// </summary>
        /// <param name="message">The message to log in the report.</param>
        private void WriteToReport(string message)
        {
            string reportFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ReportFolder);
            string reportFilePath = Path.Combine(reportFolderPath, TestResultsFile);

            // Ensure the Reports folder exists, creating it if necessary
            if (!Directory.Exists(reportFolderPath))
            {
                Directory.CreateDirectory(reportFolderPath);
            }

            // Append the message to the report file with a timestamp
            using (StreamWriter writer = new StreamWriter(reportFilePath, append: true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
            }
        }
    }

    /// <summary>
    /// Model class for mapping test credentials from a JSON file.
    /// </summary>
    public class TestCredentials
    {
        /// <summary>
        /// Holds the valid user credentials for the test.
        /// </summary>
        public UserCredentials? ValidUser { get; set; }
    }

    /// <summary>
    /// Represents the credentials required for user login.
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// The username for authentication.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The password associated with the username.
        /// </summary>
        public string? Password { get; set; }
    }
}
