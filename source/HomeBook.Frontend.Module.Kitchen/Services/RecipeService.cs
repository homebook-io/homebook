using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Module.Kitchen.Contracts;
using HomeBook.Frontend.Module.Kitchen.Mappings;
using HomeBook.Frontend.Module.Kitchen.Models;

namespace HomeBook.Frontend.Module.Kitchen.Services;

/// <inheritdoc/>
public class RecipeService(
    IAuthenticationService authenticationService,
    BackendClient backendClient) : IRecipeService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(string filter,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        RecipesListResponse? response = await backendClient.Modules.Kitchen.Recipes.GetAsync(x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        if (response is null)
            return [];

        List<RecipeDto> result = (response.Recipes ?? [])
            .Select(x => x.ToDto())
            .ToList();

        return result;
    }

    public async Task CreateRecipeAsync(string name,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        CreateRecipeRequest request = new();
        request.Name = name;

        await backendClient.Modules.Kitchen.Recipes.PostAsync(request,
            x =>
            {
                x.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken);
    }
}
