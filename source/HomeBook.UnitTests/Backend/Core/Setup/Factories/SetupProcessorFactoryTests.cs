using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using Homebook.Backend.Core.Setup;
using HomeBook.Backend.Data.Entities;
using Homebook.Backend.Core.Setup.Factories;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Core.Setup.Factories;

[TestFixture]
public class SetupProcessorFactoryTests
{
    private IDatabaseMigratorFactory _databaseMigratorFactory;
    private IHashProviderFactory _hashProviderFactory;
    private IValidator<User> _userValidator;
    private IValidator<Configuration> _configurationValidator;
    private IUpdateProcessor _updateProcessor;
    private SetupProcessorFactory _factory;

    [SetUp]
    public void SetUp()
    {
        _databaseMigratorFactory = Substitute.For<IDatabaseMigratorFactory>();
        _hashProviderFactory = Substitute.For<IHashProviderFactory>();
        _userValidator = Substitute.For<IValidator<User>>();
        _configurationValidator = Substitute.For<IValidator<Configuration>>();
        _updateProcessor = Substitute.For<IUpdateProcessor>();

        _factory = new SetupProcessorFactory(
            _databaseMigratorFactory,
            _hashProviderFactory,
            _userValidator,
            _configurationValidator,
            _updateProcessor);
    }

    [Test]
    public void Create_MultipleCallsWithSameDependencies_ShouldReturnDifferentInstances()
    {
        // Arrange
        var instances = new List<ISetupProcessor>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            instances.Add(_factory.Create());
        }

        // Assert
        instances.ShouldAllBe(instance => instance != null);
        instances.ShouldAllBe(instance => instance is SetupProcessor);

        // Verify all instances are different
        for (int i = 0; i < instances.Count; i++)
        {
            for (int j = i + 1; j < instances.Count; j++)
            {
                instances[i].ShouldNotBeSameAs(instances[j]);
            }
        }
    }
}
