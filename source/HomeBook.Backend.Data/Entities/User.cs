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

    /// <summary>
    /// the password hash of this user
    /// </summary>
    [Required]
    [StringLength(512, ErrorMessage = "The password hash cannot exceed 512 characters.")]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// the type of the password hash
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "The password hash type cannot exceed 50 characters.")]
    public required string PasswordHashType { get; set; }

    /// <summary>
    /// UTC Zeitstempel der Erstellung - wird automatisch von der Datenbank gesetzt
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC Zeitstempel der Deaktivierung (null = aktiv)
    /// </summary>
    public DateTime? Disabled { get; set; }
}
