using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public static class VersionHandler
{
    public static async Task<IResult> HandleGetVersion(
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        string? version = configuration.GetSection("Version")?.Value?.Trim();

        if (string.IsNullOrEmpty(version))
        {
            return TypedResults.InternalServerError("Service Version is not configured.");
        }

        return TypedResults.Ok(version);
    }
}
