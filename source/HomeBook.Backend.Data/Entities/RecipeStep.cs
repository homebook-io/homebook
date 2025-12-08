using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBook.Backend.Data.Entities;

[Table("RecipeSteps")]
public class RecipeStep
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid RecipeId { get; set; }
    public virtual Recipe Recipe { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Description { get; set; }

    public int? TimerDurationInSeconds { get; set; }
}
