using NUnit.Framework;

namespace API_Tests.Base
{
    /// <summary>
    /// Base class for API tests providing shared setup, teardown, and utility methods.
    /// </summary>
    public class ApiTestBase
    {
        protected HttpClient client = null!;

        // Base URL for API calls
        protected virtual string BaseUrl => "https://fakestoreapi.com/";

        /// <summary>
        /// Initializes the HttpClient and sets default headers.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(60) // Set request timeout to 60 seconds
            };

            AddDefaultHeaders();
        }

        /// <summary>
        /// Disposes the HttpClient instance to free resources after each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            client?.Dispose();
        }

        /// <summary>
        /// Adds default headers for all API requests. Can be overridden in derived classes.
        /// </summary>
        protected virtual void AddDefaultHeaders()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Writes a log message to the API test report file.
        /// </summary>
        /// <param name="message">Message to write to the report.</param>
        protected void WriteToReport(string message)
        {
            string reportFilePath = GetReportFilePath();

            try
            {
                using (StreamWriter writer = new StreamWriter(reportFilePath, append: true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to report: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the file path for the API test report and ensures the directory exists.
        /// </summary>
        /// <returns>Path to the report file.</returns>
        private string GetReportFilePath()
        {
            string reportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports");
            string reportFilePath = Path.Combine(reportFolder, "ApiTestResults.txt");

            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder); // Ensure the report directory exists.
            }

            return reportFilePath;
        }
    }
}
