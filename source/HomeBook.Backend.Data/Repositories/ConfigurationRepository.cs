using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class ConfigurationRepository(AppDbContext dbContext) : IConfigurationRepository
{
    /// <inheritdoc />
    public async Task WriteConfigurationAsync(Configuration configuration,
        CancellationToken cancellationToken = default)
    {
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
