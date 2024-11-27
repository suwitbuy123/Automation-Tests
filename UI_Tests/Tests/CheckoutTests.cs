using NUnit.Framework;
using OpenQA.Selenium;
using UI_Tests.Base;
using UI_Tests.Pages;

namespace UI_Tests.Tests
{
    public class CheckoutTests : BaseTest
    {
        [Test]
        public void CompleteCheckoutProcess()
        {
            // Arrange: Initialize page objects for Login, Product, Cart, and Checkout functionality
            var loginPage = new LoginPage(driver);
            var productPage = new ProductPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            // Act: Log in with valid credentials
            TestContext.WriteLine("Logging in as 'standard_user'.");
            loginPage.Login("standard_user", "secret_sauce");

            // Add all products to the cart
            TestContext.WriteLine("Adding all products to cart.");
            productPage.AddAllProductsToCart();

            // Navigate to the cart page
            TestContext.WriteLine("Navigating to the cart page.");
            productPage.GoToCart();

            // Proceed to the checkout process
            TestContext.WriteLine("Proceeding to checkout.");
            cartPage.Checkout();

            // Assert: Verify shipping information fields are loaded
            TestContext.WriteLine("Validating shipping information fields are present.");
            Assert.That(WaitForElement(By.Id("first-name")), Is.True,
                "Shipping information fields did not load in time.");

            // Submit shipping information with valid inputs
            TestContext.WriteLine("Submitting shipping information.");
            checkoutPage.SubmitShippingInformation("John", "Doe", "12345");

            // Assert: Ensure no input errors after submitting shipping information
            if (checkoutPage.HasInputError())
            {
                Assert.Fail("Input error detected after submitting shipping information.");
            }

            // Retrieve and validate the total price displayed
            TestContext.WriteLine("Retrieving total price.");
            string totalPrice = checkoutPage.GetTotalPrice();
            Assert.That(totalPrice, Is.Not.Empty,
                "Total price is not displayed or missing!");

            // Complete the checkout process
            TestContext.WriteLine("Completing the checkout process.");
            checkoutPage.FinishCheckout();

            // Assert: Verify that the checkout process navigates to the "checkout complete" URL
            TestContext.WriteLine("Verifying checkout completion.");
            Assert.That(driver.Url, Is.EqualTo("https://www.saucedemo.com/checkout-complete.html"),
                $"Checkout process was not completed successfully! Current URL: {driver.Url}");
        }
    }
}
