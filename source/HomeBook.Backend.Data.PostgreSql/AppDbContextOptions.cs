using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.PostgreSql;

public static class AppDbContextOptions
{
    public const string DatabaseConnectionStringName = "Database";
    public const string HistoryTableName = "__EFMigrationsHistory";
    public const string HistoryTableSchema = "dbo";

    public static void SetDbOptions(this DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString,
            x => x.MigrationsHistoryTable(HistoryTableName, HistoryTableSchema)
                .EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
    }
}
