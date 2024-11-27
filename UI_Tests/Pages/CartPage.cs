using NUnit.Framework;
using OpenQA.Selenium;

namespace UI_Tests.Pages
{
    public class CartPage
    {
        private readonly IWebDriver driver;

        // Selectors
        private readonly By CartItems = By.ClassName("inventory_item_name");
        private readonly By CheckoutButton = By.Id("checkout");

        public CartPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Verifies if a specific product is in the cart.
        /// </summary>
        /// <param name="productName">The name of the product to verify.</param>
        /// <returns>True if the product is found; otherwise, false.</returns>
        public bool VerifyProductInCart(string productName)
        {
            try
            {
                var cartItems = driver.FindElements(CartItems).Select(item => item.Text).ToList();
                TestContext.WriteLine($"Cart contains: {string.Join(", ", cartItems)}");
                return cartItems.Any(item => item.Equals(productName, StringComparison.OrdinalIgnoreCase));
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error verifying product in cart: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifies that all expected products are present in the cart.
        /// </summary>
        /// <param name="expectedProducts">The list of expected product names.</param>
        /// <returns>True if all expected products are found in the cart; otherwise, false.</returns>
        public bool VerifyAllProductsInCart(List<string> expectedProducts)
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                var cartItems = driver.FindElements(CartItems).Select(item => item.Text).ToList();
                TestContext.WriteLine($"Attempt {attempt}: Cart contains: {string.Join(", ", cartItems)}");

                var missingProducts = expectedProducts.Where(product => !cartItems.Contains(product)).ToList();

                if (!missingProducts.Any())
                {
                    TestContext.WriteLine("All expected products are present in the cart.");
                    return true;
                }

                TestContext.WriteLine($"Missing products: {string.Join(", ", missingProducts)}");
                TestContext.WriteLine("Retrying product verification...");
                Thread.Sleep(2000); // รอโหลดเพิ่มเติม
            }

            TestContext.WriteLine("Product verification failed after 3 attempts.");
            return false;
        }


        /// <summary>
        /// Retrieves the names of all items in the cart.
        /// </summary>
        /// <returns>A list of product names currently in the cart.</returns>
        public List<string> GetCartItemNames()
        {
            try
            {
                var items = driver.FindElements(CartItems);
                var productNames = items.Select(i => i.Text).ToList();
                TestContext.WriteLine($"Retrieved cart items: {string.Join(", ", productNames)}");
                return productNames;
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error retrieving cart item names: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Proceeds to the checkout process by clicking the checkout button.
        /// </summary>
        public void Checkout()
        {
            try
            {
                var checkoutButton = driver.FindElement(CheckoutButton);
                if (checkoutButton.Displayed && checkoutButton.Enabled)
                {
                    checkoutButton.Click();
                    TestContext.WriteLine("Proceeded to checkout.");
                }
                else
                {
                    throw new InvalidOperationException("Checkout button is not clickable.");
                }
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error clicking checkout button: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the total number of items in the cart.
        /// </summary>
        /// <returns>The number of items in the cart.</returns>
        public int GetCartItemCount()
        {
            try
            {
                var items = driver.FindElements(CartItems);
                var itemCount = items.Count;
                TestContext.WriteLine($"Number of items in the cart: {itemCount}");
                return itemCount;
            }
            catch (NoSuchElementException ex)
            {
                TestContext.WriteLine($"Error retrieving cart item count: {ex.Message}");
                return 0;
            }
        }
    }
}
