namespace HomeBook.Backend.Abstractions.Models.UserManagement;

public record UserInfo(Guid Id, string Username, DateTime Created, DateTime? Disabled)
{
    public DateTime Created { get; set; } = Created;
    public DateTime? Disabled { get; set; } = Disabled;
}
