using HomeBook.Backend.Abstractions.Contracts;

namespace Homebook.Backend.Core.Setup.UpdateMigrators;

/// <inheritdoc />
public class Update_1_0_10_AddInstanceData : IUpdateMigrator
{
    /// <inheritdoc />
    public string Version { get; } = "1.0.10";

    /// <inheritdoc />
    public string Description { get; } = "Add HomeBook instance data";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
    }
}
