using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Entities;

[PrimaryKey(nameof(RecipeId), nameof(IngredientId))]
public class RecipeIngredient
{
    [Required]
    public Guid RecipeId { get; set; }
    public virtual Recipe Recipe { get; set; } = null!;

    [Required]
    public Guid IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    [MaxLength(50)]
    public string? Quantity { get; set; }

    [MaxLength(20)]
    public string? Unit { get; set; }
}
