using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models.UserManagement;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup.UpdateMigrators;

/// <summary>
/// 1. updated the created timestamp for all existing users
/// </summary>
/// <param name="userProvider"></param>
public class Update_1_0_10(ILogger<Update_1_0_10> logger,
    IUserProvider userProvider) : IUpdateMigrator
{
    /// <inheritdoc />
    public string Version { get; } = "1.0.10";

    /// <inheritdoc />
    public string Description { get; } = "Add HomeBook instance data";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // update users and add created timestamp
        logger.LogInformation("Updating existing users");

        IEnumerable<UserInfo> users = await userProvider.GetAllAsync(cancellationToken);
        foreach (UserInfo userInfo in users)
        {
            userInfo.Created = DateTime.UtcNow;
            await userProvider.UpdateUserAsync(userInfo, cancellationToken);
        }
    }
}
