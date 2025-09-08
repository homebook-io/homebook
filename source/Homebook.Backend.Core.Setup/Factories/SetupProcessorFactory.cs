using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup.Factories;

/// <inheritdoc />
public class SetupProcessorFactory(
    IDatabaseMigratorFactory databaseMigratorFactory,
    IHashProviderFactory hashProviderFactory,
    ILoggerFactory loggerFactory,
    IConfiguration injectedConfiguration,
    IFileSystemService fileSystemService,
    IApplicationPathProvider applicationPathProvider) : ISetupProcessorFactory
{
    /// <inheritdoc />
    public ISetupProcessor Create() =>
        new SetupProcessor(databaseMigratorFactory,
            hashProviderFactory,
            loggerFactory,
            injectedConfiguration,
            fileSystemService,
            applicationPathProvider);
}
