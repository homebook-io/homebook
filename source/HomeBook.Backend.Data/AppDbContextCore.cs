using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data;

public class AppDbContextCore : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Configuration> Configurations { get; set; } = null!;

    public AppDbContextCore()
    {
    }

    public AppDbContextCore(DbContextOptions<AppDbContextCore> options)
        : base(options)
    {
    }
}
