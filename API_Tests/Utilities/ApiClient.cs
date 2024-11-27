using System.Text;
using System.Text.Json;

namespace API_Tests.Utilities
{
    /// <summary>
    /// Provides reusable methods for making HTTP requests (GET, POST, PUT, DELETE) to API endpoints.
    /// Ensures a consistent and centralized way to interact with APIs in tests.
    /// </summary>
    public static class ApiClient
    {
        /// <summary>
        /// Sends a GET request to the specified API endpoint.
        /// </summary>
        /// <param name="client">The HttpClient instance used to make the request.</param>
        /// <param name="endpoint">The API endpoint to send the request to.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> GetAsync(HttpClient client, string endpoint)
        {
            return await client.GetAsync(endpoint);
        }

        /// <summary>
        /// Sends a POST request with JSON-encoded data to the specified API endpoint.
        /// </summary>
        /// <param name="client">The HttpClient instance used to make the request.</param>
        /// <param name="endpoint">The API endpoint to send the request to.</param>
        /// <param name="data">The data object to be serialized and sent in the request body.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> PostAsync(HttpClient client, string endpoint, object data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await client.PostAsync(endpoint, content);
        }

        /// <summary>
        /// Sends a PUT request with JSON-encoded data to the specified API endpoint.
        /// </summary>
        /// <param name="client">The HttpClient instance used to make the request.</param>
        /// <param name="endpoint">The API endpoint to send the request to.</param>
        /// <param name="data">The data object to be serialized and sent in the request body.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> PutAsync(HttpClient client, string endpoint, object data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await client.PutAsync(endpoint, content);
        }

        /// <summary>
        /// Sends a DELETE request to the specified API endpoint.
        /// </summary>
        /// <param name="client">The HttpClient instance used to make the request.</param>
        /// <param name="endpoint">The API endpoint to send the request to.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(HttpClient client, string endpoint)
        {
            return await client.DeleteAsync(endpoint);
        }
    }
}
