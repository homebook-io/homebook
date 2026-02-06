namespace HomeBook.Backend.Abstractions.Contracts;

public interface ISearchRegistrationInitiator
{
    void AddModule(string moduleId);
    void AddServiceProvider(IServiceProvider serviceProvider);
}
