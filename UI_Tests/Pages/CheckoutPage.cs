using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UI_Tests.Pages
{
    public class CheckoutPage
    {
        private readonly IWebDriver driver;

        // Selectors for elements on the Checkout Page
        private By FirstNameField = By.Id("first-name");
        private By LastNameField = By.Id("last-name");
        private By ZipCodeField = By.Id("postal-code");
        private By ContinueButton = By.Id("continue");
        private By FinishButton = By.Id("finish");
        private By TotalPrice = By.ClassName("summary_total_label");
        private By ErrorMessage = By.ClassName("error-message-container");

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Waits for a specific element to appear within the given timeout.
        /// </summary>
        /// <param name="by">The locator for the target element.</param>
        /// <param name="timeoutInSeconds">The maximum time to wait for the element (in seconds).</param>
        /// <returns>True if the element appears; otherwise, false.</returns>
        private bool WaitForElement(By by, int timeoutInSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(d => d.FindElement(by).Displayed);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"Element with selector '{by}' not found within {timeoutInSeconds} seconds.");
                return false;
            }
        }

        /// <summary>
        /// Fills in the shipping information form and clicks the "Continue" button.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="zipCode">The ZIP code of the user's address.</param>
        public void EnterShippingInformation(string firstName, string lastName, string zipCode)
        {
            try
            {
                if (!WaitForElement(FirstNameField)) throw new InvalidOperationException("First name field not found.");
                driver.FindElement(FirstNameField).Clear();
                driver.FindElement(FirstNameField).SendKeys(firstName);

                if (!WaitForElement(LastNameField)) throw new InvalidOperationException("Last name field not found.");
                driver.FindElement(LastNameField).Clear();
                driver.FindElement(LastNameField).SendKeys(lastName);

                if (!WaitForElement(ZipCodeField)) throw new InvalidOperationException("ZIP code field not found.");
                driver.FindElement(ZipCodeField).Clear();
                driver.FindElement(ZipCodeField).SendKeys(zipCode);

                if (!WaitForElement(ContinueButton)) throw new InvalidOperationException("Continue button not found.");
                driver.FindElement(ContinueButton).Click();

                TestContext.WriteLine($"Shipping information submitted: {firstName}, {lastName}, {zipCode}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error during shipping information submission: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clicks the "Finish" button to complete the checkout process.
        /// </summary>
        /// <returns>True if the button is clicked successfully; otherwise, false.</returns>
        public bool ClickFinishButton()
        {
            try
            {
                if (!WaitForElement(FinishButton))
                {
                    TestContext.WriteLine("Finish button not found.");
                    return false;
                }

                driver.FindElement(FinishButton).Click();
                return true;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error clicking finish button: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if there is an error message displayed on the page.
        /// </summary>
        /// <returns>True if an error message is found; otherwise, false.</returns>
        public bool HasInputError()
        {
            if (WaitForElement(ErrorMessage))
            {
                var errorElement = driver.FindElement(ErrorMessage);
                TestContext.WriteLine($"Error Message Displayed: {errorElement.Text}");
                return true;
            }
            TestContext.WriteLine("No error message detected.");
            return false;
        }

        /// <summary>
        /// Attempts to retrieve the total price from the checkout summary.
        /// Retries up to 3 times if the price is not immediately available.
        /// </summary>
        /// <returns>The total price as a string. Returns an empty string if the retrieval fails.</returns>
        public string GetTotalPrice()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    if (!WaitForElement(TotalPrice))
                    {
                        TestContext.WriteLine($"Attempt {attempt}: Total price label not found.");
                        Thread.Sleep(2000); // Wait for potential UI load delays
                        continue;
                    }

                    var price = driver.FindElement(TotalPrice).Text;
                    TestContext.WriteLine($"Attempt {attempt}: Total price found: {price}");
                    return price;
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Attempt {attempt}: Error retrieving total price: {ex.Message}");
                }
            }

            TestContext.WriteLine("Total price retrieval failed after 3 attempts.");
            return string.Empty;
        }

        /// <summary>
        /// Combines the steps to enter and submit shipping information.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="zipCode">The ZIP code of the user's address.</param>
        /// <returns>True if the information is submitted successfully; otherwise, false.</returns>
        public bool SubmitShippingInformation(string firstName, string lastName, string zipCode)
        {
            try
            {
                EnterShippingInformation(firstName, lastName, zipCode);
                return true;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error during shipping information submission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Completes the checkout process by clicking the "Finish" button.
        /// Throws an exception if the button is not clicked successfully.
        /// </summary>
        public void FinishCheckout()
        {
            if (!ClickFinishButton())
            {
                throw new InvalidOperationException("Finish button not clicked.");
            }
        }
    }
}
