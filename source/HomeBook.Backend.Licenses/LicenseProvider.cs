using System.Reflection;
using AngleSharp;
using AngleSharp.Dom;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Models;

namespace HomeBook.Backend.Licenses;

public class LicenseProvider : ILicenseProvider
{
    public async Task<DependencyLicense[]> GetLicensesAsync(CancellationToken cancellationToken)
    {
        string executableLocation = Assembly.GetExecutingAssembly().Location;
        string executableDirectory = Path.GetDirectoryName(executableLocation) ?? string.Empty;
        string licensesDirectory = Path.Combine(executableDirectory, "Licenses");
        string[] filesInDirectory = Directory.GetFiles(licensesDirectory, "*.*", SearchOption.AllDirectories);

        List<DependencyLicense> licenses = [];
        foreach (string file in filesInDirectory)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string fileContent = await File.ReadAllTextAsync(file, cancellationToken);

            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            IDocument doc = await context.OpenAsync(r => r.Content(fileContent), cancellationToken);

            string bodyInnerHtml = doc.Body?.InnerHtml ?? fileContent;
            bodyInnerHtml = bodyInnerHtml.Trim()
                .Replace("id", "class");

            licenses.Add(new DependencyLicense(fileName, bodyInnerHtml));
        }

        return licenses.ToArray();
    }
}
