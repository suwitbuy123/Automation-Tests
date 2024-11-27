using NUnit.Framework;
using UI_Tests.Base;
using UI_Tests.Pages;

namespace UI_Tests.Tests
{
    public class CartValidationTests : BaseTest
    {
        [Test]
        public void ValidateCartContents()
        {
            // Arrange: Initialize page objects for Login, Product, and Cart functionality
            var loginPage = new LoginPage(driver);
            var productPage = new ProductPage(driver);
            var cartPage = new CartPage(driver);

            // Define the expected products to be validated in the cart
            var expectedProducts = new List<string>
            {
                "Sauce Labs Backpack",
                "Sauce Labs Bike Light"
            };

            // Act: Log in, add all available products to the cart, and navigate to the cart page
            loginPage.Login("standard_user", "secret_sauce");
            productPage.AddAllProductsToCart();
            productPage.GoToCart();

            // Assert: Verify all expected products are present in the cart
            bool areAllProductsInCart = cartPage.VerifyAllProductsInCart(expectedProducts);
            Assert.That(areAllProductsInCart, Is.True,
                $"Not all expected products are in the cart. Expected: {string.Join(", ", expectedProducts)}");
        }
    }
}
