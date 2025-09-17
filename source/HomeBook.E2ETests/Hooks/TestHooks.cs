using HomeBook.E2ETests.Configuration;
using HomeBook.Client;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;
using Reqnroll;

namespace HomeBook.E2ETests.Hooks;

/// <summary>
/// Reqnroll hooks for test setup and teardown
/// </summary>
[Binding]
public class TestHooks
{
    private static ApiTestConfiguration? _configuration;
    private static BackendClient? _apiClient;

    /// <summary>
    /// Setup before all tests
    /// </summary>
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        _configuration = ApiTestConfiguration.Load();

        // Create HTTP client with timeout
        HttpClient httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds)
        };

        // Create request adapter
        HttpClientRequestAdapter requestAdapter = new(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient)
        {
            BaseUrl = _configuration.ApiBaseUrl
        };

        _apiClient = new BackendClient(requestAdapter);
    }

    /// <summary>
    /// Cleanup after all tests
    /// </summary>
    [AfterTestRun]
    public static void AfterTestRun()
    {
        // Cleanup resources if needed
        _apiClient = null;
        _configuration = null;
    }

    /// <summary>
    /// Setup before each scenario
    /// </summary>
    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        // Make configuration and client available to step definitions
        scenarioContext["Configuration"] = _configuration;
        scenarioContext["ApiClient"] = _apiClient;
    }

    /// <summary>
    /// Cleanup after each scenario
    /// </summary>
    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext)
    {
        // Clear any scenario-specific data
        scenarioContext.Clear();
    }
}
