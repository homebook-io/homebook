using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.UnitTests.TestCore.Backend.Services;

public class UnitTestDbMigrator : IDatabaseMigrator
{
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public DbContext GetDbContext()
    {
        return null!;
    }

    public void ConfigureForServiceCollection(ServiceCollection services, IConfiguration configuration)
    {

    }
}
