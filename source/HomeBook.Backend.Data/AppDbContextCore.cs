using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data;

public class AppDbContextCore : DbContext
{
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Configuration> Configurations { get; set; } = null!;

    public AppDbContextCore()
    {
    }

    public AppDbContextCore(DbContextOptions<AppDbContextCore> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
