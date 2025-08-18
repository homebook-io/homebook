namespace HomeBook.Frontend.Abstractions.Contracts;

public interface ILocalizationProvider
{
    string this[string name] { get; }

    string this[string name, params object[] arguments] { get; }
}
