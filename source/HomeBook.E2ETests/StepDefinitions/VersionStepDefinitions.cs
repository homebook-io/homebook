using System.Net.Http.Json;
using HomeBook.Client;
using HomeBook.E2ETests.Configuration;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.E2ETests.StepDefinitions;

/// <summary>
/// step definitions for version api endpoint
/// </summary>
[Binding]
public class VersionStepDefinitions(ScenarioContext scenarioContext)
{
    private ApiTestConfiguration Configuration => (ApiTestConfiguration)scenarioContext["Configuration"];
    private BackendClient ApiClient => (BackendClient)scenarioContext["ApiClient"];

    [When("I request the backend version")]
    public async Task WhenIRequestTheBackendVersion()
    {
        NativeResponseHandler native = new();
        await ApiClient.Version.GetAsync(x =>
            {
                x.Options.Add(new ResponseHandlerOption
                {
                    ResponseHandler = native
                });
            },
            CancellationToken.None);

        var response = (native.Value as HttpResponseMessage);

        int? statusCode = (int?)response?.StatusCode;
        scenarioContext.Set(statusCode, "ResponseStatusCode");

        string responseContent = await response.Content.ReadFromJsonAsync<string>();
        scenarioContext.Set(responseContent, "ResponseContent");
    }

    [Then("the response should contain a valid version string")]
    public void ThenTheResponseShouldContainAValidVersionString()
    {
        string versionResponse = scenarioContext.Get<string>("ResponseContent");
        versionResponse.ShouldNotBeNullOrEmpty("Version response should not be null or empty");

        Version.TryParse(versionResponse, out var version)
            .ShouldBeTrue("Version response should be a valid version string");
        version.ShouldNotBeNull("Parsed version should not be null");
    }
}
