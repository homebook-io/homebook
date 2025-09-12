using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public static class SystemHandler
{
    public static IResult HandleGetSystemInfo([FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            string? dotnetVersion = Environment.Version.ToString();
            string? appVersion = configuration["Version"];
            string? databaseProvider = configuration["Database:Provider"];
            string deploymentType = "Docker";

            GetSystemInfoResponse response = new(dotnetVersion,
                appVersion,
                databaseProvider,
                deploymentType);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem("An error occurred while retrieving system information.", statusCode: 500);
        }
    }
}
