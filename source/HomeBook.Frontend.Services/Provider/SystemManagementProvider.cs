using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Models.System;
using HomeBook.Frontend.Services.Mappings;

namespace HomeBook.Frontend.Services.Provider;

/// <inheritdoc />
public class SystemManagementProvider(
    BackendClient backendClient,
    IAuthenticationService authenticationService) : ISystemManagementProvider
{
    /// <inheritdoc />
    public async Task<SystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default)
    {
        await IsAdminOrThrowAsync(cancellationToken);

        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        GetSystemInfoResponse? response = await backendClient.System.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);

        SystemInfo result = response.ToSystemInfo();
        return result;
    }

    private async Task IsAdminOrThrowAsync(CancellationToken cancellationToken)
    {
        bool isUserAdmin = await authenticationService.IsCurrentUserAdminAsync(cancellationToken);
        if (!isUserAdmin)
            throw new UnauthorizedAccessException("User is not authorized to access system information.");
    }
}
