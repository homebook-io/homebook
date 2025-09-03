using HomeBook.Backend.Abstractions.Contracts;

namespace Homebook.Backend.Core.Setup.UpdateMigrators;

/// <inheritdoc />
public class Update_1_0_1_AddInstanceData : IUpdateMigrator
{
    /// <inheritdoc />
    public string Version { get; } = "1.0.1";

    /// <inheritdoc />
    public string Description { get; } = "Add HomeBook instance file";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
    }
}
