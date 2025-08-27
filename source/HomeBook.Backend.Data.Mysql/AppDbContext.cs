using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Mysql;

public class AppDbContext : AppDbContextCore
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContextCore> options)
        : base(options)
    {
    }
}
