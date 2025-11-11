using System.ComponentModel.DataAnnotations;
using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Entities;

[Index(nameof(NormalizedName), IsUnique = true)]
public class Ingredient : INormalizable
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string NormalizedName { get; set; } = null!;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];

    public void Normalize(IStringNormalizer normalizer)
    {
        NormalizedName = normalizer.Normalize(Name);
    }
}
