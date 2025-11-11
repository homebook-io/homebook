using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using HomeBook.Backend.Abstractions.Contracts;

namespace HomeBook.Backend.Data.Entities;

[DebuggerDisplay("[{nameof(Recipe)}] {Name}")]
public class Recipe : INormalizable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters long.")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string NormalizedName { get; set; } = null!;

    public int? DurationMinutes { get; set; }

    [NotMapped]
    public TimeSpan? Duration
    {
        get =>
            DurationMinutes.HasValue
                ? TimeSpan.FromMinutes(DurationMinutes.Value)
                : null;

        set =>
            DurationMinutes = value.HasValue
                ? (int)value.Value.TotalMinutes
                : null;
    }

    public int? CaloriesKcal { get; set; }

    public void Normalize(IStringNormalizer normalizer)
    {
        NormalizedName = normalizer.Normalize(Name);
    }
}
