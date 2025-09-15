namespace HomeBook.E2ETests.Helpers;

/// <summary>
/// Helper class to access test user data from ScenarioContext
/// </summary>
public static class TestUserHelper
{
    /// <summary>
    /// Gets the test user ID from ScenarioContext
    /// </summary>
    /// <param name="scenarioContext">The scenario context</param>
    /// <returns>The test user ID</returns>
    /// <exception cref="InvalidOperationException">Thrown when test user ID is not found</exception>
    public static Guid GetTestUserId(ScenarioContext scenarioContext)
    {
        if (scenarioContext.TryGetValue("TestUserId", out object? userIdObj) && userIdObj is Guid userId)
        {
            return userId;
        }

        throw new InvalidOperationException("TestUserId not found in ScenarioContext. Ensure the scenario is tagged with @NeedsTestUser");
    }

    /// <summary>
    /// Gets the test username from ScenarioContext
    /// </summary>
    /// <param name="scenarioContext">The scenario context</param>
    /// <returns>The test username</returns>
    /// <exception cref="InvalidOperationException">Thrown when test username is not found</exception>
    public static string GetTestUsername(ScenarioContext scenarioContext)
    {
        if (scenarioContext.TryGetValue("TestUsername", out object? usernameObj) && usernameObj is string username)
        {
            return username;
        }

        throw new InvalidOperationException("TestUsername not found in ScenarioContext. Ensure the scenario is tagged with @NeedsTestUser");
    }

    /// <summary>
    /// Gets the test password from ScenarioContext
    /// </summary>
    /// <param name="scenarioContext">The scenario context</param>
    /// <returns>The test password</returns>
    /// <exception cref="InvalidOperationException">Thrown when test password is not found</exception>
    public static string GetTestPassword(ScenarioContext scenarioContext)
    {
        if (scenarioContext.TryGetValue("TestPassword", out object? passwordObj) && passwordObj is string password)
        {
            return password;
        }

        throw new InvalidOperationException("TestPassword not found in ScenarioContext. Ensure the scenario is tagged with @NeedsTestUser");
    }

    /// <summary>
    /// Tries to get the test user ID from ScenarioContext
    /// </summary>
    /// <param name="scenarioContext">The scenario context</param>
    /// <param name="userId">The test user ID if found</param>
    /// <returns>True if test user ID was found, false otherwise</returns>
    public static bool TryGetTestUserId(ScenarioContext scenarioContext, out Guid userId)
    {
        if (scenarioContext.TryGetValue("TestUserId", out object? userIdObj) && userIdObj is Guid id)
        {
            userId = id;
            return true;
        }

        userId = default;
        return false;
    }
}
