using HomeBook.Frontend.Abstractions.Contracts;
using Microsoft.Extensions.Localization;

namespace HomeBook.Frontend.Provider;

public class LocalizationProvider(IStringLocalizer loc)
    : ILocalizationProvider
{
    public string this[string name] => loc[name].Value;

    public string this[string name, params object[] arguments] => loc[name, arguments].Value;
}
