using HomeBook.Client;
using HomeBook.E2ETests.Configuration;

namespace HomeBook.E2ETests.StepDefinitions;

/// <summary>
/// shared step definitions
/// </summary>
[Binding]
public class SharedStepDefinitions(ScenarioContext scenarioContext)
{
    private ApiTestConfiguration Configuration => (ApiTestConfiguration)scenarioContext["Configuration"];
    private BackendClient ApiClient => (BackendClient)scenarioContext["ApiClient"];

    [Given(@"the API is available at the configured base URL")]
    public void GivenTheApiIsAvailableAtTheConfiguredBaseUrl()
    {
        // API client is already configured in the hooks
        ApiClient.ShouldNotBeNull("API client should be configured");
        Configuration.ApiBaseUrl.ShouldNotBeNullOrEmpty("API base URL should be configured");
    }

    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        int? actualStatusCode = scenarioContext.Get<int?>("ResponseStatusCode");

        actualStatusCode.ShouldNotBeNull("Status code should be set in the scenario context");
        actualStatusCode.ShouldBe(expectedStatusCode, $"Status code should be {expectedStatusCode}");
    }
}
