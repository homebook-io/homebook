using Microsoft.Extensions.Configuration;

namespace HomeBook.Frontend.Modules.Abstractions;

public interface IModuleWidgetRegistration
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    static abstract void RegisterWidgets(IWidgetBuilder builder,
        IConfiguration configuration);
}
