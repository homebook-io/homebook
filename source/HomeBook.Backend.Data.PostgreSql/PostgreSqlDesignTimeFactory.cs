using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeBook.Backend.Data.PostgreSql;

public sealed class PostgreSqlDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=homebook;Username=hb;Password=hb",
            npg => npg.MigrationsAssembly(typeof(PostgreSqlDesignTimeFactory).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options);
    }
}
