using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.E2ETests.Configuration;

namespace HomeBook.E2ETests.StepDefinitions;

/// <summary>
/// step definitions for account api endpoint
/// </summary>
[Binding]
public class AccountStepDefinitions(ScenarioContext scenarioContext)
{
    private ApiTestConfiguration Configuration => (ApiTestConfiguration)scenarioContext["Configuration"];
    private BackendClient ApiClient => (BackendClient)scenarioContext["ApiClient"];
    private LoginRequest? _loginRequest;
    private LoginResponse? _loginResponse;
    private Exception? _lastException;
    private string? _logoutResponse;
    private string? _accessToken;

    [Given(@"I have valid test user credentials")]
    public void GivenIHaveValidTestUserCredentials()
    {
        Configuration.TestUser.Username.ShouldNotBeNullOrEmpty("Test username should be configured");
        Configuration.TestUser.Password.ShouldNotBeNullOrEmpty("Test password should be configured");
    }

    [Given(@"I have valid user credentials")]
    public void GivenIHaveValidUserCredentials()
    {
        _loginRequest = new LoginRequest
        {
            Username = Configuration.TestUser.Username,
            Password = Configuration.TestUser.Password
        };
    }

    [When(@"I attempt to login")]
    public async Task WhenIAttemptToLogin()
    {
        try
        {
            _lastException = null;
            _loginResponse = await ApiClient.Account.Login.PostAsync(_loginRequest!);

            scenarioContext.Set(_loginResponse?.Token, "AccessToken");
        }
        catch (Exception ex)
        {
            _lastException = ex;
            _loginResponse = null;
        }
    }

    [When(@"I attempt to logout")]
    public async Task WhenIAttemptToLogout()
    {
        try
        {
            string accessToken = scenarioContext.Get<string>("AccessToken")
                                 ?? throw new InvalidOperationException(
                                     "No access token found in context. Ensure you are logged in before logging out.");

            _lastException = null;
            _logoutResponse = await ApiClient.Account.Logout.PostAsync(x =>
                {
                    x.Headers.Add("Authorization", $"Bearer {accessToken}");
                },
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _lastException = ex;
        }
    }

    [Then(@"the login should succeed")]
    public void ThenTheLoginShouldSucceed()
    {
        _lastException.ShouldBeNull($"Login should not throw exception: {_lastException?.Message}");
        _loginResponse.ShouldNotBeNull("Login response should not be null");
    }

    [Then(@"an access token should be provided")]
    public void ThenAnAccessTokenShouldBeProvided()
    {
        _loginResponse.ShouldNotBeNull("Login response is required");

        _accessToken = _loginResponse.Token;
        _accessToken.ShouldNotBeNullOrEmpty("Access token should be provided in successful login response");
    }

    [Then(@"user information should be returned")]
    public void ThenUserInformationShouldBeReturned()
    {
        _loginResponse.ShouldNotBeNull("Login response is required");
        _loginResponse!.Username.ShouldNotBeNullOrEmpty("Username should be provided in login response");
        _loginResponse.UserId.ShouldNotBeNull("User ID should be provided in login response");
    }

    [Then(@"the logout should succeed")]
    public void ThenTheLogoutShouldSucceed()
    {
        _lastException.ShouldBeNull($"Logout should not throw exception: {_lastException?.Message}");
        _logoutResponse.ShouldBe("Logout successful", "Logout response should confirm successful logout");
    }
}
