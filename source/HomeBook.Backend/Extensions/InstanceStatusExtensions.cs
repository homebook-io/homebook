using HomeBook.Backend.Abstractions;

namespace HomeBook.Backend.Extensions;

public static class InstanceStatusExtensions
{
    /// <summary>
    /// get the current instance status based on the configuration value Database:Provider
    /// if the value is null or empty, the instance is in SETUP mode, otherwise homebook is RUNNING
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static InstanceStatus GetCurrentInstanceStatus(this IConfiguration configuration)
    {
        bool isGithubWorkflow = configuration.GetValue<bool>("GITHUB_WORKFLOW");
        if (isGithubWorkflow)
            return InstanceStatus.RUNNING;

        string? databaseProvider = configuration["Database:Provider"];
        return databaseProvider switch
        {
            null or "" => InstanceStatus.SETUP,
            _ => InstanceStatus.RUNNING
        };
    }
}
