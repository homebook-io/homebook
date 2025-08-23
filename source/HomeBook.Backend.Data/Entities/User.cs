using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBook.Backend.Data.Entities;

[Table("Users")]
public class User
{
    /// <summary>
    /// the id of this Product Team
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// the username of this user
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "The username must be between 5 and 20 characters long.")]
    public required string Username { get; set; }
}
