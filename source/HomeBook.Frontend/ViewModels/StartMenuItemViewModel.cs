using HomeBook.Frontend.Abstractions.Contracts;

namespace HomeBook.Frontend.ViewModels;

public class StartMenuItemViewModel(string title,
    string caption,
    string url,
    string icon,
    string color) : IStartMenuItem
{
    public string Title { get; } = title;
    public string Caption { get; } = caption;
    public string Url { get; } = url;
    public string Icon { get; } = icon;
    public string Color { get; } = color;
}
