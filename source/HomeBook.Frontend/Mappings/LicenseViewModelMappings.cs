using HomeBook.Client.Models;
using HomeBook.Frontend.Models.Setup;

namespace HomeBook.Frontend.Mappings;

public static class LicenseViewModelMappings
{
    public static LicenseViewModel ToViewModel(this DependencyLicense license)
    {
        return new LicenseViewModel(license.Name, license.MarkdownContent);
    }
}
