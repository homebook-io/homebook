using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Core.DataProvider;

/// <inheritdoc />
public class InstanceConfigurationProvider(
    IConfigurationRepository configurationRepository,
    IValidator<Configuration> configurationValidator) : IInstanceConfigurationProvider
{
    private static readonly string HOMEBOOK_INSTANCE_NAME = "HOMEBOOK_INSTANCE_NAME";
    private static readonly string HOMEBOOK_INSTANCE_DEFAULT_LANG = "HOMEBOOK_INSTANCE_DEFAULT_LANG";

    /// <inheritdoc />
    public async Task SetHomeBookInstanceNameAsync(string instanceName,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<string> GetHomeBookInstanceNameAsync(CancellationToken cancellationToken = default)
    {
        Configuration? instanceName = await configurationRepository
            .GetConfigurationByKeyAsync(HOMEBOOK_INSTANCE_NAME,
                cancellationToken);
        return instanceName?.Value ?? string.Empty;
    }

    public async Task SetHomeBookInstanceDefaultLanguageAsync(string defaultLanguage,
        CancellationToken cancellationToken = default)
    {
        Configuration config = new()
        {
            Key = HOMEBOOK_INSTANCE_NAME,
            Value = defaultLanguage
        };
        await configurationValidator.ValidateAndThrowAsync<Configuration>(config,
            cancellationToken: cancellationToken);

        await configurationRepository.WriteConfigurationAsync(config, cancellationToken);
    }

    public async Task<string?> GetHomeBookInstanceDefaultLanguageAsync(CancellationToken cancellationToken = default)
    {
        Configuration? defaultLanguage = await configurationRepository
            .GetConfigurationByKeyAsync(HOMEBOOK_INSTANCE_DEFAULT_LANG,
                cancellationToken);
        return defaultLanguage?.Value ?? null;
    }
}
