namespace HomeBook.Backend.Abstractions.Contracts;

public interface ISearchRegistrationFactory
{
    ISearchProvider CreateSearchProvider();
}
