using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Data.Interceptors;

public class NormalizationInterceptor(
    ILogger<NormalizationInterceptor> logger,
    IStringNormalizer normalizer) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        DbContext dbContext = eventData.Context!;

        var allEntries = dbContext.ChangeTracker.Entries();
        IEnumerable<INormalizable> entries = allEntries
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity)
            .OfType<INormalizable>();

        foreach (INormalizable entity in entries)
            entity.Normalize(normalizer);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        DbContext dbContext = eventData.Context!;

        var allEntries = dbContext.ChangeTracker.Entries();
        IEnumerable<INormalizable> entries = allEntries
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity)
            .OfType<INormalizable>();

        foreach (INormalizable entity in entries)
            entity.Normalize(normalizer);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
