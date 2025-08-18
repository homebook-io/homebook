using HomeBook.Backend.Abstractions.Models;

namespace HomeBook.Backend.Abstractions;

public interface ILicenseProvider
{
    Task<DependencyLicense[]> GetLicensesAsync(CancellationToken cancellationToken);
}
