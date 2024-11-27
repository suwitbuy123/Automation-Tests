namespace UI_Tests.Base
{
    /// <summary>
    /// Represents a collection of user credentials for testing purposes.
    /// </summary>
    public class TestCredentials
    {
        /// <summary>
        /// A list of user credentials used for authentication and validation in tests.
        /// </summary>
        public List<UserCredentials> Users { get; set; } = new List<UserCredentials>();
    }

    /// <summary>
    /// Represents the credentials and expected outcome for a single user.
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// The username of the test user.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password associated with the test user's account.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The URL that is expected after successful login.
        /// </summary>
        public string ExpectedUrl { get; set; } = string.Empty;
    }
}

