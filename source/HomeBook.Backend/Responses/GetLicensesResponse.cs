using HomeBook.Backend.Abstractions.Models;

namespace HomeBook.Backend.Responses;

public record GetLicensesResponse(DependencyLicense[] Licenses);
