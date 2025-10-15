using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Core.Icons;
using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Module.Finances.Pages;

public partial class Overview : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        MenuService.UpdateMenuItems([
            new MenuItem("+Übersicht", "/Finances", HomeBookIcons.Icons8.Windows11.Outline.MoneyBagEuro),
            new MenuItem("+Konten", "/Finances/BankAccounts", HomeBookIcons.Icons8.Windows11.Outline.CardMagnetic),
            new MenuItem("+Einstellungen", "/Finances/Settings", HomeBookIcons.Icons8.Windows11.Outline.Gear2),
        ]);

        await base.OnInitializedAsync();
    }
}
