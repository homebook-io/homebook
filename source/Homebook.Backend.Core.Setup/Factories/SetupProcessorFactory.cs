using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Entities;

namespace Homebook.Backend.Core.Setup.Factories;

/// <inheritdoc />
public class SetupProcessorFactory(
    IDatabaseMigratorFactory databaseMigratorFactory,
    ISetupConfigurationProvider setupConfigurationProvider,
    IHashProviderFactory hashProviderFactory,
    IValidator<User> userValidator,
    IValidator<Configuration> configurationValidator) : ISetupProcessorFactory
{
    /// <inheritdoc />
    public ISetupProcessor Create() =>
        new SetupProcessor(databaseMigratorFactory,
            setupConfigurationProvider,
            hashProviderFactory,
            userValidator,
            configurationValidator);
}
