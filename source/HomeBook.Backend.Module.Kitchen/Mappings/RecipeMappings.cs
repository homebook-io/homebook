using HomeBook.Backend.Module.Kitchen.Models;
using HomeBook.Backend.Module.Kitchen.Responses;

namespace HomeBook.Backend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeDto ToDto(this Data.Entities.Recipe recipe)
    {
        return new RecipeDto(
            recipe.Id,
            recipe.Name,
            recipe.NormalizedName);
    }

    public static RecipeResponse ToResponse(this RecipeDto recipe)
    {
        return new RecipeResponse(recipe.Id,
            recipe.Name,
            recipe.NormalizedName);
    }
}
