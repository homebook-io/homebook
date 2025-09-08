using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup.Factories;

/// <inheritdoc />
public class SetupProcessorFactory(
    IDatabaseMigratorFactory databaseMigratorFactory,
    IHashProviderFactory hashProviderFactory,
    IValidator<User> userValidator,
    IValidator<Configuration> configurationValidator,
    ILoggerFactory loggerFactory,
    IConfiguration injectedConfiguration,
    IFileSystemService fileSystemService,
    IApplicationPathProvider applicationPathProvider) : ISetupProcessorFactory
{
    /// <inheritdoc />
    public ISetupProcessor Create() =>
        new SetupProcessor(databaseMigratorFactory,
            hashProviderFactory,
            userValidator,
            configurationValidator,
            loggerFactory,
            injectedConfiguration,
            fileSystemService,
            applicationPathProvider);
}
