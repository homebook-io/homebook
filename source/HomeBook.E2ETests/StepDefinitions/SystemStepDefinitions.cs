using System.Net.Http.Json;
using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.E2ETests.Configuration;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.E2ETests.StepDefinitions;

/// <summary>
/// step definitions for system api endpoint
/// </summary>
[Binding]
public class SystemStepDefinitions(ScenarioContext scenarioContext)
{
    private ApiTestConfiguration Configuration => (ApiTestConfiguration)scenarioContext["Configuration"];
    private BackendClient ApiClient => (BackendClient)scenarioContext["ApiClient"];

    [When("I request HTTP GET system")]
    public async Task WhenIRequestHttpgetSystem()
    {
        string accessToken = scenarioContext.Get<string>("AccessToken");
        NativeResponseHandler native = new();
        await ApiClient.System.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {accessToken}");
                x.Options.Add(new ResponseHandlerOption
                {
                    ResponseHandler = native
                });
            },
            CancellationToken.None);

        var response = (native.Value as HttpResponseMessage);

        int? statusCode = (int?)response?.StatusCode;
        scenarioContext.Set(statusCode, "ResponseStatusCode");

        try
        {
            GetSystemInfoResponse? responseContent = await response.Content.ReadFromJsonAsync<GetSystemInfoResponse?>();
            scenarioContext.Set(responseContent, "ResponseContent");
        }
        catch (Exception err)
        {
            scenarioContext.Set(err, "LastException");
            scenarioContext.Set((GetSystemInfoResponse?)null, "ResponseContent");
        }
    }

    [When("I request HTTP GET system users")]
    public async Task WhenIRequestHttpgetSystemUsers()
    {
        NativeResponseHandler native = new();
        await ApiClient.System.Users.GetAsync(x =>
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

        try
        {
            GetUsersResponse? responseContent = await response.Content.ReadFromJsonAsync<GetUsersResponse?>();
            scenarioContext.Set(responseContent, "ResponseContent");
        }
        catch (Exception err)
        {
            scenarioContext.Set(err, "LastException");
            scenarioContext.Set((GetUsersResponse?)null, "ResponseContent");
        }
    }

    [When("I request HTTP PUT system users disable")]
    public async Task WhenIRequestHttpputSystemUsersDisable()
    {
        NativeResponseHandler native = new();
        Guid userId = scenarioContext.Get<Guid>("TestUserId");
        await ApiClient.System.Users[userId].Disable.PutAsync(x =>
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

        try
        {
            string? responseContent = await response.Content.ReadFromJsonAsync<string?>();
            scenarioContext.Set(responseContent, "ResponseContent");
        }
        catch (Exception err)
        {
            scenarioContext.Set(err, "LastException");
            scenarioContext.Set((string?)null, "ResponseContent");
        }
    }

    [When("I request HTTP PUT system users enable")]
    public async Task WhenIRequestHttpputSystemUsersEnable()
    {
        NativeResponseHandler native = new();
        Guid userId = scenarioContext.Get<Guid>("TestUserId");
        await ApiClient.System.Users[userId].Enable.PutAsync(x =>
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

        try
        {
            string? responseContent = await response.Content.ReadFromJsonAsync<string?>();
            scenarioContext.Set(responseContent, "ResponseContent");
        }
        catch (Exception err)
        {
            scenarioContext.Set(err, "LastException");
            scenarioContext.Set((string?)null, "ResponseContent");
        }
    }

    [Then("the system info should be valid")]
    public void ThenTheSystemInfoShouldBeValid()
    {
        GetSystemInfoResponse? response = scenarioContext.Get<GetSystemInfoResponse>("ResponseContent");

        response.ShouldNotBeNull();
        response.DotnetRuntimeVersion.ShouldNotBeNullOrEmpty();
        response.AppVersion.ShouldNotBeNullOrEmpty();
        response.DatabaseProvider.ShouldNotBeNullOrEmpty();
        response.DeploymentType.ShouldNotBeNullOrEmpty();
    }
}
