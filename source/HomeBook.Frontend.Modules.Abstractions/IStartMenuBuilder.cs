namespace HomeBook.Frontend.Modules.Abstractions;

public interface IStartMenuBuilder
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="Title"></param>
    /// <param name="Caption"></param>
    /// <param name="Url"></param>
    /// <param name="Icon"></param>
    /// <param name="Color"></param>
    /// <returns></returns>
    IStartMenuBuilder AddStartMenu(string Title,
        string Caption,
        string Url,
        string Icon,
        string Color);
}
