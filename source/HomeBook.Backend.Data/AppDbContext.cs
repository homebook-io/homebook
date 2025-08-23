using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Configuration> Configurations { get; set; } = null!;

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
