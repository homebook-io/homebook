using Microsoft.Extensions.Configuration;

namespace HomeBook.E2ETests.Configuration;

/// <summary>
/// Configuration class for API testing settings
/// </summary>
public class ApiTestConfiguration
{
    /// <summary>
    /// Base URL for the API
    /// </summary>
    public string ApiBaseUrl { get; set; } = "";

    /// <summary>
    /// Test user credentials
    /// </summary>
    public TestUserCredentials TestUser { get; set; } = new();

    /// <summary>
    /// HTTP client timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Load configuration from appsettings or environment variables
    /// </summary>
    public static ApiTestConfiguration Load()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        ApiTestConfiguration config = new();
        configuration.Bind("ApiTestConfiguration", config);

        return config;
    }
}

/// <summary>
/// Test user credentials for authentication tests
/// </summary>
public class TestUserCredentials
{
    /// <summary>
    /// Username for test user
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    /// Password for test user
    /// </summary>
    public string Password { get; set; } = "";
}
