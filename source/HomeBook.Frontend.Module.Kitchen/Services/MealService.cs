using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Module.Kitchen.Contracts;
using HomeBook.Frontend.Module.Kitchen.Mappings;
using HomeBook.Frontend.Module.Kitchen.Models;

namespace HomeBook.Frontend.Module.Kitchen.Services;

/// <inheritdoc/>
public class MealService(
    IAuthenticationService authenticationService,
    BackendClient backendClient) : IMealService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<RecipeDto>> GetMealsAsync(string filter,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? token = await authenticationService.GetTokenAsync(cancellationToken);
        RecipesListResponse? response = await backendClient.Kitchen.Recipes.GetAsync(x =>
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
}
