using System.Diagnostics;

namespace HomeBook.Frontend.Abstractions.Models;

/// <summary>
/// definition of a start menu item
/// </summary>
/// <param name="Title"></param>
/// <param name="Caption"></param>
/// <param name="Url"></param>
/// <param name="Icon"></param>
/// <param name="Color"></param>
[DebuggerDisplay("{Title}")]
public record StartMenuItem(string Title,
    string Caption,
    string Url,
    string Icon,
    string Color);
