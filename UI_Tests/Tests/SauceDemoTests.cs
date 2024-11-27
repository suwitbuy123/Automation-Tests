using NUnit.Framework;
using Newtonsoft.Json;
using UI_Tests.Base;
using UI_Tests.Pages;

namespace UI_Tests.Tests
{
    public class SauceDemoTests : BaseTest
    {
        // Class to represent test credentials loaded from a JSON file
        private class TestCredentials
        {
            public List<string> Usernames { get; set; } = new List<string>(); // List of test usernames
            public string Password { get; set; } = string.Empty; // Common password for all test users
        }

        private TestCredentials credentials = new TestCredentials();

        // Paths for test reports and credentials
        private const string ReportFolderPath = "/Users/suwit/Desktop/AutomationAssignment/UI_Tests/Reports";
        private const string TestResultsFile = "TestResults.txt";

        [SetUp]
        public void LoadCredentials()
        {
            try
            {
                // Load credentials from a JSON file located in the specified path
                var jsonFilePath = "/Users/suwit/Desktop/AutomationAssignment/UI_Tests/TestData/TestCredentials.json";

                // Verify that the file exists
                if (!File.Exists(jsonFilePath)) throw new FileNotFoundException($"Test credentials file not found at path: {jsonFilePath}");

                // Deserialize the JSON file into the TestCredentials object
                var jsonData = File.ReadAllText(jsonFilePath);
                credentials = JsonConvert.DeserializeObject<TestCredentials>(jsonData) ?? new TestCredentials();

                // Ensure the report folder exists, and create it if necessary
                if (!Directory.Exists(ReportFolderPath))
                {
                    Directory.CreateDirectory(ReportFolderPath);
                }

                // Initialize the Test Results file with a header
                File.WriteAllText(Path.Combine(ReportFolderPath, TestResultsFile), "Test Results:\n\n");
            }
            catch (Exception ex)
            {
                // Fail the test if loading credentials or initializing reports fails
                Assert.Fail($"Failed to load test credentials or initialize report: {ex.Message}");
            }
        }

        [Test]
        public void EndToEndWorkflowTest()
        {
            // Load the expected product list, either hardcoded or from JSON
            var expectedProducts = LoadExpectedProducts();

            // Loop through each test user from the credentials
            foreach (var username in credentials.Usernames)
            {
                TestContext.WriteLine($"Running test for user: {username}");
                bool loginSuccessful = false;

                try
                {
                    // Step 1: Open browser and login
                    driver.Navigate().GoToUrl("https://www.saucedemo.com/");
                    var loginPage = new LoginPage(driver);
                    loginSuccessful = loginPage.Login(username, credentials.Password);

                    // Verify login success
                    if (!loginSuccessful)
                    {
                        LogTestResult(username, $"Login failed for user '{username}'.");
                        continue;
                    }

                    // Step 2: Add all products to cart
                    var productPage = new ProductPage(driver);
                    productPage.AddAllProductsToCart();
                    productPage.GoToCart();

                    // Step 3: Verify all expected products are in the cart
                    var cartPage = new CartPage(driver);
                    if (!cartPage.VerifyAllProductsInCart(expectedProducts))
                    {
                        LogTestResult(username, $"Product verification failed for user '{username}'. Expected: {string.Join(", ", expectedProducts)}");
                        continue;
                    }

                    // Step 4: Proceed to checkout and validate shipping
                    cartPage.Checkout();
                    var checkoutPage = new CheckoutPage(driver);
                    if (!checkoutPage.SubmitShippingInformation("John", "Doe", "12345"))
                    {
                        LogTestResult(username, $"Shipping information submission failed for user '{username}'.");
                        continue;
                    }

                    // Step 5: Verify the total price is displayed
                    string totalPrice = checkoutPage.GetTotalPrice();
                    if (string.IsNullOrEmpty(totalPrice))
                    {
                        LogTestResult(username, $"Total price is missing for user '{username}'.");
                        continue;
                    }

                    // Step 6: Complete checkout and validate final state
                    if (!checkoutPage.ClickFinishButton())
                    {
                        LogTestResult(username, $"Checkout process not completed for user '{username}'. Current URL: {driver.Url}");
                        continue;
                    }

                    // Validate the checkout completion URL
                    Assert.That(driver.Url, Is.EqualTo("https://www.saucedemo.com/checkout-complete.html"),
                        $"Checkout process not completed successfully for user '{username}'.");

                    // Log success if all steps pass
                    LogTestResult(username, $"Test passed successfully for user '{username}'.");
                }
                catch (Exception ex)
                {
                    // Log errors encountered during the test
                    LogTestResult(username, $"Test failed for user '{username}' with error: {ex.Message}");
                }
                finally
                {
                    // Logout at the end of each test
                    if (loginSuccessful)
                    {
                        try
                        {
                            new LoginPage(driver).Logout();
                        }
                        catch (Exception ex)
                        {
                            TestContext.WriteLine($"Logout failed for user '{username}': {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the expected product list for verification.
        /// </summary>
        /// <returns>List of expected product names.</returns>
        private List<string> LoadExpectedProducts()
        {
            // Return a hardcoded list of product names
            return new List<string>
            {
                "Sauce Labs Backpack",
                "Sauce Labs Bike Light",
                "Sauce Labs Bolt T-Shirt",
                "Sauce Labs Fleece Jacket",
                "Sauce Labs Onesie",
                "Test.allTheThings() T-Shirt (Red)"
            };
        }

        /// <summary>
        /// Logs the test result for a specific user to the report file.
        /// </summary>
        /// <param name="username">The username of the test user.</param>
        /// <param name="resultMessage">The message describing the test result.</param>
        private void LogTestResult(string username, string resultMessage)
        {
            try
            {
                // Construct the full path to the log file
                string logFilePath = Path.Combine(ReportFolderPath, TestResultsFile);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: User '{username}' - {resultMessage}\n";

                // Append the result to the log file
                File.AppendAllText(logFilePath, logEntry);
            }
            catch (Exception ex)
            {
                // Log to the test context if writing to the file fails
                TestContext.WriteLine($"Failed to log test result for user '{username}': {ex.Message}");
            }
        }
    }
}
