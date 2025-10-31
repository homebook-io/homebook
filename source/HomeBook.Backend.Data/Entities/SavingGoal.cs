using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBook.Backend.Data.Entities;

[Table("SavingGoals")]
public class SavingGoal
{
    /// <summary>
    /// The id of this Product Team
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// the id of the user this preference belongs to
    /// </summary>
    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    /// <summary>
    /// navigation property to the user
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// the name of this saving goal
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Key must be between 3 and 50 characters long.")]
    public required string Name { get; set; }

    /// <summary>
    /// the accent color of this saving goal
    /// </summary>
    [Required]
    [StringLength(7, ErrorMessage = "Color must be a valid HEX color code and 7 characters long (#000000).")]
    public required string Color { get; set; }

    /// <summary>
    /// the target amount of this saving goal
    /// </summary>
    [Required]
    public required decimal TargetAmount { get; set; }

    /// <summary>
    /// the current amount of this saving goal
    /// </summary>
    [Required]
    public required decimal CurrentAmount { get; set; }

    /// <summary>
    /// the monthly payment towards this saving goal
    /// </summary>
    [Required]
    public required decimal MonthlyPayment { get; set; }

    /// <summary>
    /// the target date of this saving goal
    /// </summary>
    public DateTime? TargetDate { get; set; }
}
