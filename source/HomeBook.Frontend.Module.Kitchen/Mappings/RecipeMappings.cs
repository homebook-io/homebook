using HomeBook.Client.Models;
using HomeBook.Frontend.Module.Kitchen.Models;
using HomeBook.Frontend.Module.Kitchen.ViewModels;

namespace HomeBook.Frontend.Module.Kitchen.Mappings;

public static class RecipeMappings
{
    public static RecipeDetailViewModel ToViewModel(this RecipeDetailDto recipe)
    {
        int? durationInMinutes = recipe.DurationWorkingMinutes
                                 + recipe.DurationCookingMinutes
                                 + recipe.DurationRestingMinutes;
        TimeSpan? duration = durationInMinutes.HasValue
            ? TimeSpan.FromMinutes(durationInMinutes.Value)
            : null;

        return new RecipeDetailViewModel
        {
            Id = recipe.Id,
            Username = recipe.Username,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            NumberOfServings = recipe.Servings ?? 1,
            CaloriesKcal = recipe.CaloriesKcal,
            Duration = duration,
            DurationWorkingMinutes = recipe.DurationWorkingMinutes.HasValue
                ? TimeSpan.FromMinutes(recipe.DurationWorkingMinutes.Value)
                : null,
            DurationCookingMinutes = recipe.DurationCookingMinutes.HasValue
                ? TimeSpan.FromMinutes(recipe.DurationCookingMinutes.Value)
                : null,
            DurationRestingMinutes = recipe.DurationRestingMinutes.HasValue
                ? TimeSpan.FromMinutes(recipe.DurationRestingMinutes.Value)
                : null,
            Ingredients = recipe.Ingredients
                .Select(x => new IngredientViewModel
                {
                    Name = x.Name,
                    Quantity = x.Quantity.HasValue ? Convert.ToDecimal(x.Quantity.Value) : null,
                    Unit = x.Unit,
                    AdditionalText = null
                })
                .ToList(),
            Steps = recipe.Steps
                .OrderBy(x => x.Position)
                .Select(x => new StepViewModel
                {
                    Description = x.Description,
                    TimerDurationInSeconds = x.TimerDurationInSeconds
                })
                .ToList(),
            Image = TestImageMappings.PlaceholderImage,
            Source = recipe.Source,
            Comments = recipe.Comments
        };
    }

    public static RecipeViewModel ToViewModel(this RecipeDto recipe)
    {
        TimeSpan? duration = recipe.DurationInMinutes.HasValue
            ? TimeSpan.FromMinutes(recipe.DurationInMinutes.Value)
            : null;

        return new RecipeViewModel
        {
            Id = recipe.Id,
            Username = recipe.Username,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            CaloriesKcal = recipe.CaloriesKcal,
            Duration = duration,
            Ingredients = recipe.Ingredients,
            Image = TestImageMappings.PlaceholderImage
        };
    }

    public static RecipeDto ToDto(this HomeBook.Client.Models.RecipeResponse r) =>
        new(
            r.Id!.Value,
            r.Username!,
            r.Name!,
            r.Description!,
            r.Servings,
            r.CaloriesKcal,
            r.DurationCookingMinutes,
            "");

    public static RecipeDetailDto ToDto(this HomeBook.Client.Models.RecipeDetailResponse r) =>
        new(
            r.Id!.Value,
            r.Username!,
            r.Name!,
            r.NormalizedName!,
            r.Description!,
            r.Servings,
            (r.Ingredients ?? []).Select(x => x.ToDto()).ToArray(),
            (r.Steps ?? []).Select(x => x.ToDto()).ToArray(),
            r.DurationWorkingMinutes,
            r.DurationCookingMinutes,
            r.DurationRestingMinutes,
            r.CaloriesKcal,
            r.Comments!,
            r.Source!);

    public static RecipeIngredientDto ToDto(this RecipeIngredientResponse r) =>
        new(r.Name!,
            r.Quantity,
            r.Unit);

    public static RecipeStepDto ToDto(this RecipeStepResponse r) =>
        new(r.Description!,
            r.Position!.Value,
            r.TimerDurationInSeconds);

    public static CreateRecipeIngredientRequest ToRequest(this RecipeIngredientDto dto) =>
        new()
        {
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
        };

    public static CreateRecipeStepRequest ToRequest(this RecipeStepDto dto) =>
        new()
        {
            Description = dto.Description,
            Position = dto.Position,
            TimerDurationInSeconds = dto.TimerDurationInSeconds
        };

    public static RecipeIngredientDto ToDto(this IngredientViewModel dto) =>
        new(dto.Name,
            dto.Quantity.HasValue ? Convert.ToDouble(dto.Quantity.Value) : null,
            dto.Unit);

    public static RecipeStepDto ToDto(this StepViewModel dto,
        int position) =>
        new(dto.Description,
            position,
            dto.TimerDurationInSeconds);
}
