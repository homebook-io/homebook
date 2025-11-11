using HomeBook.Backend.Core.Kitchen.Models;
using HomeBook.Backend.DTOs.Responses.Kitchen;

namespace HomeBook.Backend.Mappings;

public static class MealMappings
{
    public static RecipeResponse ToResponse(this RecipeDto recipe)
    {
        return new RecipeResponse(recipe.Id,
            recipe.Name,
            recipe.NormalizedName);
    }
}
