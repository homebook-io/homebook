using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeBook.Backend.Data.Mysql;

public class MySqlDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AppDbContextCore> optionsBuilder = new();
        optionsBuilder.SetDbOptions("server=localhost:3306;uid=hb;pwd=hb;database=homebook;");

        return new AppDbContext(optionsBuilder.Options);
    }
}
