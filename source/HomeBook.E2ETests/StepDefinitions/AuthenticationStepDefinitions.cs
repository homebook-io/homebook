using HomeBook.Client.Models;
using HomeBook.E2ETests.Configuration;
using HomeBook.Client;

namespace HomeBook.E2ETests.StepDefinitions;

// /// <summary>
// /// Step definitions for authentication feature tests using Reqnroll
// /// </summary>
// [Binding]
// public class AuthenticationStepDefinitions(ScenarioContext scenarioContext)
// {
    // private ApiTestConfiguration Configuration => (ApiTestConfiguration)scenarioContext["Configuration"];
    // private BackendClient ApiClient => (BackendClient)scenarioContext["ApiClient"];

    // private TimeSpan _loginDuration;

    // [Given(@"I have invalid user credentials")]
    // public void GivenIHaveInvalidUserCredentials()
    // {
    //     _loginRequest = new LoginRequest
    //     {
    //         Username = "invalid@example.com",
    //         Password = "wrongpassword"
    //     };
    // }
    //
    // [Given(@"I have empty user credentials")]
    // public void GivenIHaveEmptyUserCredentials()
    // {
    //     _loginRequest = new LoginRequest
    //     {
    //         Username = "",
    //         Password = ""
    //     };
    // }
    //
    // [Given(@"I am logged in")]
    // public async Task GivenIAmLoggedIn()
    // {
    //     GivenIHaveValidUserCredentials();
    //     await WhenIAttemptToLogin();
    //     ThenTheLoginShouldSucceed();
    // }
    //
    // [Given(@"I have credentials with SQL injection attempt")]
    // public void GivenIHaveCredentialsWithSqlInjectionAttempt()
    // {
    //     _loginRequest = new LoginRequest
    //     {
    //         Username = "'; DROP TABLE Users; --",
    //         Password = "' OR '1'='1"
    //     };
    // }
    //
    // [Given(@"I have whitespace only credentials")]
    // public void GivenIHaveWhitespaceOnlyCredentials()
    // {
    //     _loginRequest = new LoginRequest
    //     {
    //         Username = "   ",
    //         Password = "   "
    //     };
    // }
    //
    // [When(@"I measure the login time")]
    // public async Task WhenIMeasureTheLoginTime()
    // {
    //     DateTime startTime = DateTime.UtcNow;
    //
    //     try
    //     {
    //         _lastException = null;
    //         _loginResponse = await ApiClient.Account.Login.PostAsync(_loginRequest!);
    //     }
    //     catch (Exception ex)
    //     {
    //         _lastException = ex;
    //     }
    //
    //     _loginDuration = DateTime.UtcNow - startTime;
    // }
    //
    // [When(@"I perform multiple login attempts")]
    // public async Task WhenIPerformMultipleLoginAttempts()
    // {
    //     int attemptCount = 3;
    //     List<bool> attemptResults = new();
    //
    //     for (int i = 0; i < attemptCount; i++)
    //     {
    //         try
    //         {
    //             LoginResponse? response = await ApiClient.Account.Login.PostAsync(_loginRequest!);
    //             attemptResults.Add(response != null);
    //         }
    //         catch
    //         {
    //             attemptResults.Add(false);
    //         }
    //     }
    //
    //     // Store results for validation
    //     _loginResponse = attemptResults.All(success => success) ? new LoginResponse() : null;
    // }
    //
    // [Then(@"the login should fail")]
    // public void ThenTheLoginShouldFail()
    // {
    //     _lastException.ShouldNotBeNull("Login should throw an exception for invalid credentials");
    //     _loginResponse.ShouldBeNull("Login response should be null when login fails");
    // }
    //
    // [Then(@"the login should fail with validation error")]
    // public void ThenTheLoginShouldFailWithValidationError()
    // {
    //     _lastException.ShouldNotBeNull("Login should throw an exception for empty credentials");
    //     _loginResponse.ShouldBeNull("Login response should be null when validation fails");
    //
    //     // Check if it's a validation error (HttpValidationProblemDetails)
    //     _lastException.ShouldBeOfType<Exception>("Exception should indicate validation error");
    // }
    //
    // [Then(@"the access token should be cleared")]
    // public void ThenTheAccessTokenShouldBeCleared()
    // {
    //     _currentAccessToken.ShouldBeNull("Access token should be cleared after logout");
    // }
    //
    // [Then(@"the login should complete within acceptable time")]
    // public void ThenTheLoginShouldCompleteWithinAcceptableTime()
    // {
    //     _loginDuration.TotalSeconds.ShouldBeLessThan(5.0,
    //         $"Login should complete within 5 seconds, but took {_loginDuration.TotalSeconds:F2} seconds");
    //     _lastException.ShouldBeNull("Login should not throw exception");
    //     _loginResponse.ShouldNotBeNull("Login response should not be null");
    // }
    //
    // [Then(@"the login should fail safely")]
    // public void ThenTheLoginShouldFailSafely()
    // {
    //     _lastException.ShouldNotBeNull("SQL injection attempt should be rejected");
    //     _loginResponse.ShouldBeNull("Login response should be null for malicious input");
    // }
    //
    // [Then(@"all attempts should succeed")]
    // public void ThenAllAttemptsShouldSucceed()
    // {
    //     _loginResponse.ShouldNotBeNull("All login attempts should succeed");
    // }
// }
