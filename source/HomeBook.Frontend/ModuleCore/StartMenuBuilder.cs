using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Modules.Abstractions;
using HomeBook.Frontend.ViewModels;

namespace HomeBook.Frontend.ModuleCore;

/// <inheritdoc />
public class StartMenuBuilder : IStartMenuBuilder, IStartMenuRegistrator
{
    private readonly List<StartMenuItem> _startMenuItems = new();

    /// <inheritdoc />
    public IStartMenuBuilder AddStartMenu(string Title,
        string Caption,
        string Url,
        string Icon,
        string Color)
    {
        _startMenuItems.Add(new StartMenuItem(Title,
            Caption,
            Url,
            Icon,
            Color));

        return this;
    }

    public StartMenuItem[] GetStartMenuItems() => _startMenuItems.ToArray();

    public void RegisterStartMenuItems(IServiceCollection services,
        IConfiguration configuration)
    {
        StartMenuItem[] menuItems = GetStartMenuItems();
        foreach (StartMenuItem smi in menuItems)
        {
            services.AddSingleton<IStartMenuItem>(x => new StartMenuItemViewModel(smi.Title,
                smi.Caption,
                smi.Url,
                smi.Icon,
                smi.Color));
        }
    }
}
