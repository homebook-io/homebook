using HomeBook.Backend.Mappings;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public static class PlatformHandler
{
    public static IResult HandleGetLocales([FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            var availableLocales = new List<string>
            {
                "en-GB",
                "en-US",
                "de-DE",
                "fr-FR"
            };

            LocaleResponse[] localeResponse = availableLocales
                .Select(x => x.ToLocalResponse())
                .OrderBy(x => x.Name)
                .ToArray();
            GetLocalesResponse response = new(localeResponse);

            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while loading available locales", statusCode: 500);
        }
    }
}
