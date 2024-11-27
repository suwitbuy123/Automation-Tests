namespace API_Tests.Utilities
{
    /// <summary>
    /// Provides logging functionality for API tests, allowing messages to be written to a log file and optionally to the console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The file path where log messages will be stored.
        /// </summary>
        private static readonly string logFilePath = "Reports/ApiTestResults.txt";

        /// <summary>
        /// Logs a message by appending it to a file and optionally writing it to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void WriteToReport(string message)
        {
            Console.WriteLine(message); // Optional: Log to console for immediate feedback
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}"); // Append the message to the log file
        }
    }
}
