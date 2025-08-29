using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Setup;
using HomeBook.Backend.Core.DataProvider.UserManagement;
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
    ISetupConfigurationProvider setupConfigurationProvider,
    IHashProviderFactory hashProviderFactory,
    IValidator<User> userValidator) : ISetupProcessor
{
    /// <inheritdoc />
    public async Task ProcessAsync(IConfiguration configuration,
        string? homebookUserName,
        string? homebookUserPassword,
        CancellationToken cancellationToken = default)
    {
        // load specific database type provider
        string databaseType = configuration["Database:Provider"]!;
        IDatabaseMigrator databaseMigrator = databaseMigratorFactory.CreateMigrator(databaseType);

        // 1. create database structure via migrations
        await databaseMigrator.MigrateAsync(cancellationToken);

        // 2. create user
        string? adminUsername = homebookUserName
                                ?? setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_USER_NAME);
        string? adminPassword = homebookUserPassword
                                ?? setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_USER_PASSWORD);
        if (string.IsNullOrEmpty(adminUsername)
            || string.IsNullOrEmpty(adminPassword))
            throw new SetupException("homebook username or password is not set");

        // Get the database context using the new method
        ServiceCollection services = new();
        databaseMigrator.ConfigureForServiceCollection(services, configuration);

        // Create a temporary service collection to get the repository
        services.AddTransient<IUserRepository, UserRepository>();

        var serviceProvider = services.BuildServiceProvider();
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

        UserProvider userProvider = new(userRepository,
            hashProviderFactory,
            userValidator);

        bool userExists = await userProvider.ContainsUserAsync(adminUsername, cancellationToken);
        if (userExists)
            throw new SetupException("a user with the given username already exists");

        await userProvider.CreateUserAsync(adminUsername,
                adminPassword,
                cancellationToken);
    }
}
