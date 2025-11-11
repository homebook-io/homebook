using HomeBook.Backend.Core.Kitchen.Models;

namespace HomeBook.Backend.Core.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeDto ToDto(this Data.Entities.Recipe recipe)
    {
        return new RecipeDto(
            recipe.Id,
            recipe.Name,
            recipe.NormalizedName);
    }
}
