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
    public Task<ISetupProcessor> CreateAsync(CancellationToken cancellationToken = default)
    {
        ISetupProcessor setupProcessor = new SetupProcessor(databaseMigratorFactory,
            setupConfigurationProvider,
            hashProviderFactory,
            userValidator,
            configurationValidator);
        return Task.FromResult(setupProcessor);
    }
}
