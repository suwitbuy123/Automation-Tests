using System.Diagnostics;
using System.Net;
using NUnit.Framework;
using API_Tests.Base;
using API_Tests.Utilities;

namespace API_Tests.Performance
{
    /// <summary>
    /// Contains performance tests for API endpoints to validate response time and reliability.
    /// </summary>
    public class PerformanceTests : ApiTestBase
    {
        /// <summary>
        /// Validates the performance of the "Get All Products" API.
        /// Ensures that the API responds successfully (HTTP 200) within an acceptable time threshold.
        /// </summary>
        [Test]
        public async Task GetAllProducts_Performance_ShouldBeFast()
        {
            // Start a stopwatch to measure response time
            var stopwatch = Stopwatch.StartNew();

            var response = await ApiClient.GetAsync(client, "products");

            stopwatch.Stop();

            // Log elapsed time for debugging
            Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds}ms");

            // Assert HTTP Status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "GetAllProducts failed!");

            // Assert performance threshold
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(3000), // Adjusted threshold
                $"API took too long: {stopwatch.ElapsedMilliseconds}ms");

            WriteToReport($"GetAllProducts_Performance: Completed in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
