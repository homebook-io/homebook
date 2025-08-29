using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Core.DataProvider.UserManagement;

/// <inheritdoc />
public class ConfigurationProvider(
    IConfigurationRepository configurationRepository,
    IValidator<Configuration> configurationValidator) : IConfigurationProvider
{
    private static readonly string HOMEBOOK_INSTANCE_NAME = "HOMEBOOK_INSTANCE_NAME";

    /// <inheritdoc />
    public async Task WriteHomeBookInstanceNameAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        Configuration config = new()
        {
            Key = HOMEBOOK_INSTANCE_NAME,
            Value = instanceName
        };
        await configurationValidator.ValidateAndThrowAsync<Configuration>(config,
            cancellationToken: cancellationToken);

        await configurationRepository.WriteConfigurationAsync(config, cancellationToken);
    }
}
