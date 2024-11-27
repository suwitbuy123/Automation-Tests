using NUnit.Framework;
using OpenQA.Selenium;
using UI_Tests.Base;
using UI_Tests.Pages;

namespace UI_Tests.Tests
{
    public class AddToCartTests : BaseTest
    {
        [Test]
        public void AddAllProductsToCart()
        {
            // Arrange: Initialize page objects for Login, Product, and Cart functionality
            var loginPage = new LoginPage(driver);
            var productPage = new ProductPage(driver);
            var cartPage = new CartPage(driver);

            // Define the list of products expected to be added to the cart
            var expectedProducts = new List<string>
            {
                "Sauce Labs Backpack",
                "Sauce Labs Bike Light",
                "Sauce Labs Bolt T-Shirt",
                "Sauce Labs Fleece Jacket",
                "Sauce Labs Onesie",
                "Test.allTheThings() T-Shirt (Red)"
            };

            // Act: Perform login, add all products to the cart, and navigate to the cart page
            loginPage.Login("standard_user", "secret_sauce");
            productPage.AddAllProductsToCart();
            productPage.GoToCart();

            // Assert: Verify all expected products are present in the cart
            Assert.That(cartPage.VerifyAllProductsInCart(expectedProducts), Is.True,
                "Not all products were added to the cart!");

            // Assert: Validate that the cart badge displays the correct number of products
            var cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge")).Text;
            Assert.That(cartBadge, Is.EqualTo("6"), "The cart badge does not show the correct number of products!");
        }
    }
}
