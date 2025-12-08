using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Entities;

[PrimaryKey(nameof(RecipeId), nameof(IngredientId))]
[Table("Recipe2RecipeIngredient")]
public class Recipe2RecipeIngredient
{
    [Required]
    public Guid RecipeId { get; set; }
    public virtual Recipe Recipe { get; set; } = null!;

    [Required]
    public Guid IngredientId { get; set; }
    public virtual RecipeIngredient RecipeIngredient { get; set; } = null!;

    public double? Quantity { get; set; }

    [MaxLength(20)]
    public string? Unit { get; set; }
}
