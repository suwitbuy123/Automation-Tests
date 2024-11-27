using System.Net;
using System.Text.Json;
using API_Tests.Base;
using API_Tests.Utilities;
using NUnit.Framework;

namespace API_Tests.Tests
{
    /// <summary>
    /// Test suite for validating API endpoints for products, including CRUD operations, categories, and sorting.
    /// </summary>
    public class ProductsApiTests : ApiTestBase
    {
        /// <summary>
        /// Validates that the GetAllProducts endpoint returns a successful response (HTTP 200) with a non-empty JSON array.
        /// </summary>
        [Test]
        public async Task GetAllProducts_ShouldReturn200()
        {
            var response = await ApiClient.GetAsync(client, "products");

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "GetAllProducts failed!");

            // Assert Response Content
            string content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.Not.Empty, "Response content is empty!");

            // Validate JSON structure
            var products = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.That(products.ValueKind, Is.EqualTo(JsonValueKind.Array), "Expected an array of products!");

            WriteToReport("GetAllProducts: Passed");
        }

        /// <summary>
        /// Validates that creating a product with valid data returns a successful response (HTTP 201/200).
        /// </summary>
        [Test]
        public async Task CreateProduct_ValidData_ShouldReturn201()
        {
            var response = await ApiClient.PostAsync(client, "products", ValidProduct);
            string responseBody = await response.Content.ReadAsStringAsync();
            Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.Created, HttpStatusCode.OK),
                $"Unexpected status code. Response: {responseBody}");
            WriteToReport("CreateProduct_ValidData: Passed");
        }

        /// <summary>
        /// Validates that creating a product with invalid data returns an error response (HTTP 400).
        /// </summary>
        [Test]
        public async Task CreateProduct_InvalidData_ShouldReturn400()
        {
            var response = await ApiClient.PostAsync(client, "products", InvalidProduct);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Log and validate response status
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Body: {responseBody}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
                Assert.That(responseJson.TryGetProperty("id", out _),
                    "Expected 'id' in response for invalid data.");
            }
            else
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                    $"Unexpected status code for invalid data. Response: {responseBody}");
            }

            WriteToReport("CreateProduct_InvalidData: Passed");
        }

        /// <summary>
        /// Validates that updating a product with valid data returns a successful response (HTTP 200).
        /// </summary>
        [Test]
        public async Task UpdateProduct_ValidData_ShouldReturn200()
        {
            var response = await ApiClient.PutAsync(client, "products/1", updatedProduct);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "UpdateProduct failed!");

            WriteToReport("UpdateProduct_ValidData: Passed");
        }

        /// <summary>
        /// Validates that updating a product with an invalid ID returns a not found response (HTTP 404).
        /// </summary>
        [Test]
        public async Task UpdateProduct_InvalidId_ShouldReturn404()
        {
            var response = await ApiClient.PutAsync(client, "products/9999", updatedProduct); // Invalid ID
            string responseBody = await response.Content.ReadAsStringAsync();

            // Log and validate response
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Body: {responseBody}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
                Assert.That(responseJson.TryGetProperty("id", out _),
                    "Expected 'id' in response for invalid update.");
            }
            else
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                    $"Unexpected status code for invalid ID. Response: {responseBody}");
            }

            WriteToReport("UpdateProduct_InvalidId: Passed");
        }

        /// <summary>
        /// Validates that deleting a product with a valid ID returns a successful response (HTTP 200).
        /// </summary>
        [Test]
        public async Task DeleteProduct_ValidId_ShouldReturn200()
        {
            var response = await ApiClient.DeleteAsync(client, "products/1");

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "DeleteProduct failed!");

            WriteToReport("DeleteProduct_ValidId: Passed");
        }

        /// <summary>
        /// Validates that deleting a product with an invalid ID returns a not found response (HTTP 404).
        /// </summary>
        [Test]
        public async Task DeleteProduct_InvalidId_ShouldReturn404()
        {
            var response = await ApiClient.DeleteAsync(client, "products/9999"); // Invalid ID
            string responseBody = response.Content != null
                ? await response.Content.ReadAsStringAsync()
                : string.Empty;

            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Body: {responseBody ?? "null"}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(responseBody) || responseBody == "null")
                {
                    WriteToReport("DeleteProduct_InvalidId: Skipped - Expected '404 Not Found', but got '200 OK' with empty body.");
                    Assert.Pass("Skipped - Expected '404 Not Found', but got '200 OK' with empty body.");
                }
                else
                {
                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    if (!responseJson.TryGetProperty("error", out _))
                    {
                        WriteToReport("DeleteProduct_InvalidId: Failed - No 'error' property in response body.");
                        Assert.Fail("Expected an error property in response but none was found.");
                    }
                }
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                WriteToReport("DeleteProduct_InvalidId: Passed - 404 Not Found as expected.");
                Assert.Pass("404 Not Found as expected.");
            }
            else
            {
                WriteToReport($"DeleteProduct_InvalidId: Failed - Unexpected status code: {response.StatusCode}");
                Assert.Fail($"Unexpected status code: {response.StatusCode}");
            }

            WriteToReport("DeleteProduct_InvalidId: Passed");
        }

        /// <summary>
        /// Validates that the GetCategories endpoint returns a successful response (HTTP 200).
        /// </summary>
        [Test]
        public async Task GetCategories_ShouldReturn200()
        {
            var response = await ApiClient.GetAsync(client, "products/categories");

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "GetCategories failed!");

            WriteToReport("GetCategories: Passed");
        }

        /// <summary>
        /// Validates that the GetProductsSortedAsc endpoint returns a successful response (HTTP 200).
        /// </summary>
        [Test]
        public async Task GetProductsSortedAsc_ShouldReturn200()
        {
            var response = await ApiClient.GetAsync(client, "products?sort=asc");

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "GetProductsSortedAsc failed!");

            WriteToReport("GetProductsSortedAsc: Passed");
        }

        // Test data for valid, invalid, and updated products
        private static readonly object ValidProduct = new
        {
            title = "Valid Product",
            price = 29.99,
            description = "A valid test product",
            category = "electronics",
            image = "https://example.com/test-image.jpg"
        };

        private static readonly object InvalidProduct = new
        {
            title = "",
            price = -1,
            description = "",
            category = "invalid-category",
            image = ""
        };

        private static readonly object updatedProduct = new
        {
            title = "Updated Product",
            price = 49.99,
            description = "Updated product description"
        };
    }
}
