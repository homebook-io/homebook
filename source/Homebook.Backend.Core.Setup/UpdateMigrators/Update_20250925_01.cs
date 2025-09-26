using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup.UpdateMigrators;

public class Update_20250925_01(
    ILogger<Update_20250925_01> logger,
    IInstanceConfigurationProvider instanceConfigurationProvider) : IUpdateMigrator
{
    /// <inheritdoc />
    public Version Version { get; } = new(1, 0, 96);

    /// <inheritdoc />
    public string Description { get; } = "Add IsAdmin to oldest homebook user";

    private const string DefaultLanguage = "EN";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string? existingDefaultLanguage = await instanceConfigurationProvider
            .GetHomeBookInstanceDefaultLanguageAsync(cancellationToken);

        if (!string.IsNullOrEmpty(existingDefaultLanguage))
            return;

        // update users and add created timestamp
        logger.LogInformation("Updating Default Language");

        await instanceConfigurationProvider.SetHomeBookInstanceDefaultLanguageAsync(DefaultLanguage,
            cancellationToken);
    }
}
