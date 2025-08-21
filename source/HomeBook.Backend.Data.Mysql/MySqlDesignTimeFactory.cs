using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeBook.Backend.Data.Mysql;

public class MySqlDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySQL("server=localhost:3306;uid=hb;pwd=hb;database=homebook;",
            mysql => mysql.MigrationsAssembly(typeof(MySqlDesignTimeFactory).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options);
    }
}
