using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Mysql;

public static class AppDbContextOptions
{
    public const string HistoryTableName = "__EFMigrationsHistory";
    public const string HistoryTableSchema = "hb";

    public static void SetDbOptions(this DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        string migrationAssembly = typeof(AppDbContext).Namespace ?? string.Empty;
        optionsBuilder.UseMySQL(connectionString,
            x => x.MigrationsHistoryTable(HistoryTableName, HistoryTableSchema)
                .EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                .MigrationsAssembly(migrationAssembly)
        );
    }
}
