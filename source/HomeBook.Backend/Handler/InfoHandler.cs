using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public static class InfoHandler
{
    public static async Task<IResult> HandleGetInstanceInfo(
        [FromServices] IInstanceConfigurationProvider instanceConfigurationProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string instanceName = await instanceConfigurationProvider
                .GetHomeBookInstanceNameAsync(cancellationToken);
            string? instanceDefaultName = await instanceConfigurationProvider
                .GetHomeBookInstanceDefaultLanguageAsync(cancellationToken);

            GetInstanceInfoResponse response = new(instanceName,
                (instanceDefaultName ?? string.Empty));
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while retrieving instance information.", statusCode: 500);
        }
    }

    public static async Task<IResult> HandleGetInstanceName(
        [FromServices] IInstanceConfigurationProvider instanceConfigurationProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string? instanceName = await instanceConfigurationProvider.GetHomeBookInstanceNameAsync(cancellationToken);

            return TypedResults.Ok(instanceName);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while retrieving instance name.", statusCode: 500);
        }
    }

    public static async Task<IResult> HandleGetInstanceDefaultLanguage(
        [FromServices] IInstanceConfigurationProvider instanceConfigurationProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string? instanceName = await instanceConfigurationProvider.GetHomeBookInstanceNameAsync(cancellationToken);

            return TypedResults.Ok(instanceName);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while retrieving instance name.", statusCode: 500);
        }
    }
}
