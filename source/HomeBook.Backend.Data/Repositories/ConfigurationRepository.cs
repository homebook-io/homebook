using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class ConfigurationRepository(IDbContextFactory<AppDbContext> factory) : IConfigurationRepository
{
    /// <inheritdoc />
    public async Task WriteConfigurationAsync(Configuration configuration,
        CancellationToken cancellationToken = default)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        Configuration? existingConfiguration = dbContext.Set<Configuration>().FirstOrDefault();

        if (existingConfiguration is null)
        {
            dbContext.Add(configuration);
        }
        else
        {
            existingConfiguration.Key = configuration.Key;
            existingConfiguration.Value = configuration.Value;
            dbContext.Update(existingConfiguration);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
