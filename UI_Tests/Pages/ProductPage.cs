using NUnit.Framework;
using OpenQA.Selenium;

namespace UI_Tests.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver driver;

        // Selectors for interacting with elements on the product page
        private readonly By AddToCartButtons = By.CssSelector(".btn_inventory");
        private readonly By ProductNames = By.ClassName("inventory_item_name");
        private readonly By CartIcon = By.Id("shopping_cart_container");

        public ProductPage(IWebDriver driver)
        {
            this.driver = driver; // Initialize WebDriver instance for browser interaction
        }

        /// <summary>
        /// Adds all products displayed on the product page to the shopping cart.
        /// Logs the count of added products or throws an exception if no products are found.
        /// </summary>
        public void AddAllProductsToCart()
        {
            try
            {
                // Retrieve all "Add to Cart" buttons
                var addToCartButtons = driver.FindElements(AddToCartButtons);

                if (addToCartButtons.Count == 0)
                {
                    TestContext.WriteLine("No products found to add to cart.");
                    throw new NoSuchElementException("No products found on the page.");
                }

                // Click each "Add to Cart" button if it is displayed and enabled
                foreach (var button in addToCartButtons)
                {
                    if (button.Displayed && button.Enabled)
                    {
                        button.Click();
                        TestContext.WriteLine("Clicked Add to Cart button.");
                    }
                }
                TestContext.WriteLine($"Added {addToCartButtons.Count} products to the cart.");
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error while adding products to cart: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifies if all expected products are present in the shopping cart.
        /// Compares the product names in the cart with the expected list.
        /// </summary>
        /// <param name="expectedProductNames">List of expected product names.</param>
        /// <returns>True if all expected products are present, false otherwise.</returns>
        public bool VerifyAllProductsInCart(List<string> expectedProductNames)
        {
            try
            {
                // Navigate to the cart page
                GoToCart();

                // Retrieve product names from the cart
                var cartItems = driver.FindElements(By.ClassName("inventory_item_name"));
                var cartProductNames = cartItems.Select(item => item.Text).ToList();

                TestContext.WriteLine($"Cart contains: {string.Join(", ", cartProductNames)}");

                // Check if all expected products are present in the cart
                foreach (var expectedName in expectedProductNames)
                {
                    if (!cartProductNames.Contains(expectedName))
                    {
                        TestContext.WriteLine($"Missing product: {expectedName}");
                        return false;
                    }
                }

                return true;
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error verifying products in cart: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the list of product names displayed on the product page.
        /// Logs the names of products found or returns an empty list if none are found.
        /// </summary>
        /// <returns>List of product names on the page.</returns>
        public List<string> GetProductNames()
        {
            try
            {
                // Retrieve all product name elements
                var productItems = driver.FindElements(ProductNames);
                var productNames = productItems.Select(item => item.Text).ToList();

                TestContext.WriteLine($"Products on page: {string.Join(", ", productNames)}");
                return productNames;
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error retrieving product names: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Navigates to the shopping cart page by clicking on the cart icon.
        /// Throws an exception if the cart icon is not displayed or found.
        /// </summary>
        public void GoToCart()
        {
            try
            {
                // Check if the cart icon is visible and clickable
                if (driver.FindElement(CartIcon).Displayed)
                {
                    driver.FindElement(CartIcon).Click();
                    TestContext.WriteLine("Navigated to the cart page.");
                }
                else
                {
                    throw new InvalidOperationException("Cart icon is not displayed.");
                }
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error navigating to cart: {ex.Message}");
                throw;
            }
        }
    }
}
