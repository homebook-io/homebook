using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Core.DataProvider;
using Homebook.Backend.Core.Setup.Exceptions;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Homebook.Backend.Core.Setup;

/// <inheritdoc />
public class SetupProcessor(
    IDatabaseMigratorFactory databaseMigratorFactory,
    IHashProviderFactory hashProviderFactory,
    IValidator<User> userValidator,
    IValidator<Configuration> configurationValidator,
    IUpdateProcessor updateProcessor) : ISetupProcessor
{
    /// <inheritdoc />
    public async Task ProcessAsync(IConfiguration configuration,
        SetupConfiguration setupConfiguration,
        CancellationToken cancellationToken = default)
    {
        // load specific database type provider
        string? databaseType = configuration["Database:Provider"];
        if (string.IsNullOrEmpty(databaseType))
            throw new SetupException("database provider is not configured");

        IDatabaseMigrator databaseMigrator = databaseMigratorFactory.CreateMigrator(databaseType);

        // 1. create database structure via migrations
        await databaseMigrator.MigrateAsync(cancellationToken);

        // 2. create user
        string? adminUsername = setupConfiguration.HomebookUserName;
        string? adminPassword = setupConfiguration.HomebookUserPassword;
        if (string.IsNullOrEmpty(adminUsername)
            || string.IsNullOrEmpty(adminPassword))
            throw new SetupException("homebook username or password is not set");

        // Get the database context using the new method
        ServiceCollection services = new();
        databaseMigrator.ConfigureForServiceCollection(services, configuration);

        // Create a temporary service collection to get the repository
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IUserRepository userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        UserProvider userProvider = new(userRepository,
            hashProviderFactory,
            userValidator);

        await userProvider.CreateUserAsync(adminUsername,
            adminPassword,
            cancellationToken);

        // 3. write homebook configuration
        string? configurationName = setupConfiguration.HomebookConfigurationName;
        if (string.IsNullOrEmpty(configurationName))
            throw new SetupException("homebook configuration name is not set");

        IConfigurationRepository configurationRepository =
            serviceProvider.GetRequiredService<IConfigurationRepository>();
        ConfigurationProvider configurationProvider = new(configurationRepository,
            configurationValidator);
        await configurationProvider.WriteHomeBookInstanceNameAsync(configurationName, cancellationToken);

        // 4. execute available updates
        await updateProcessor.ProcessAsync(cancellationToken);
    }
}
