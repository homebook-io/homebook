using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.E2ETests.Configuration;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;

namespace HomeBook.E2ETests.Hooks;

[Binding]
public class TestUserHooks(ScenarioContext scenarioContext)
{
    private static ApiTestConfiguration? _configuration;
    private static BackendClient? _apiClient;
    private static string? _accessToken;
    private static Guid? _testUserId;

    private static ApiTestConfiguration Configuration => _configuration ??= ApiTestConfiguration.Load();

    private static BackendClient ApiClient
    {
        get
        {
            if (_apiClient == null)
            {
                // Create HTTP client with timeout
                HttpClient httpClient = new()
                {
                    Timeout = TimeSpan.FromSeconds(Configuration.TimeoutSeconds)
                };

                // Create request adapter
                HttpClientRequestAdapter requestAdapter = new(
                    new AnonymousAuthenticationProvider(),
                    httpClient: httpClient)
                {
                    BaseUrl = Configuration.ApiBaseUrl
                };

                _apiClient = new BackendClient(requestAdapter);
            }

            return _apiClient;
        }
    }

    [BeforeFeature("NeedsTestUser")]
    public static async Task CreateUser()
    {
        // 1. login via admin account
        await LoginAdminAsync();

        // 2. create user via admin account
        string username = Configuration.TestUserToCreate.Username;
        string password = Configuration.TestUserToCreate.Password;
        bool isAdmin = false;
        _testUserId = await CreateTestUserAsync(username, password, isAdmin);

        // 3. logout admin account
        await LogoutAdminAsync();
    }

    [BeforeScenario("NeedsTestUser")]
    public void StoreTestUserIdInScenarioContext()
    {
        if (_testUserId.HasValue)
        {
            scenarioContext["TestUserId"] = _testUserId.Value;
            scenarioContext["TestUsername"] = Configuration.TestUserToCreate.Username;
            scenarioContext["TestPassword"] = Configuration.TestUserToCreate.Password;
        }
    }

    [AfterFeature("NeedsTestUser")]
    public static async Task DeleteUser()
    {
        if (_testUserId == null)
            return;

        // 1. login via admin account
        await LoginAdminAsync();

        // 2. delete user via admin account
        await DeleteTestUserAsync(_testUserId.Value);

        // 3. logout admin account
        await LogoutAdminAsync();

        // reset for next test run
        _testUserId = null;
    }

    private static async Task LoginAdminAsync()
    {
        LoginResponse? adminLoginResponse = await ApiClient.Account.Login.PostAsync(new LoginRequest
            {
                Username = Configuration.AdminUser.Username,
                Password = Configuration.AdminUser.Password
            },
            cancellationToken: CancellationToken.None);

        _accessToken = adminLoginResponse?.Token
                       ?? throw new InvalidOperationException("Login response or token is null");
    }

    private static async Task LogoutAdminAsync()
    {
        if (_accessToken == null)
            return;

        await ApiClient.Account.Logout.PostAsync(requestConfiguration: x =>
            {
                x.Headers.Add("Authorization", $"Bearer {_accessToken}");
            },
            cancellationToken: CancellationToken.None);
    }

    private static async Task<Guid> CreateTestUserAsync(string username,
        string password,
        bool isAdmin = false)
    {
        if (_accessToken == null)
            throw new InvalidOperationException("Access token is null");

        CreateUserResponse? response = await ApiClient.System.Users.PostAsync(new CreateUserRequest
            {
                IsAdmin = isAdmin,
                Username = username,
                Password = password
            },
            requestConfiguration: x =>
            {
                x.Headers.Add("Authorization", $"Bearer {_accessToken}");
            },
            cancellationToken: CancellationToken.None);

        return response?.UserId ?? throw new InvalidOperationException("UserId is null in CreateUserResponse");
    }

    private static async Task DeleteTestUserAsync(Guid testUserId)
    {
        if (_accessToken == null)
            throw new InvalidOperationException("Access token is null");

        await ApiClient.System.Users.DeleteAsync(new DeleteUserRequest
            {
                UserId = testUserId
            },
            requestConfiguration: x =>
            {
                x.Headers.Add("Authorization", $"Bearer {_accessToken}");
            },
            cancellationToken: CancellationToken.None);
    }
}
