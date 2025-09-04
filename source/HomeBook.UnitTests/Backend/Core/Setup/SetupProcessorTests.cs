using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Abstractions.Setup;
using HomeBook.Backend.Core.DataProvider.Validators;
using Homebook.Backend.Core.Setup;
using Homebook.Backend.Core.Setup.Exceptions;
using HomeBook.Backend.Data.PostgreSql;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Core.Setup;

[TestFixture]
public class SetupProcessorTests
{
    private CancellationToken _cancellationToken;
    private ILogger<SetupProcessor> _logger;
    private IDatabaseMigratorFactory _databaseMigratorFactory;
    private IHashProviderFactory _hashProviderFactory;
    private IUpdateProcessor _updateProcessor;
    private SetupProcessor _instance;

    [SetUp]
    public void SetUp()
    {
        _cancellationToken = CancellationToken.None;
        var factory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                })
                .SetMinimumLevel(LogLevel.Debug);
        });
        _logger = factory.CreateLogger<SetupProcessor>();
        _databaseMigratorFactory = Substitute.For<IDatabaseMigratorFactory>();
        _hashProviderFactory = Substitute.For<IHashProviderFactory>();
        _updateProcessor = Substitute.For<IUpdateProcessor>();
        _instance = new SetupProcessor(_databaseMigratorFactory,
            _hashProviderFactory,
            new UserValidator(),
            new ConfigurationValidator(),
            _updateProcessor);
    }

    [Test]
    public async Task ProcessAsync_WithoutDatabaseType_Throws()
    {
        // Arrange
        var setupConfiguration = new SetupConfiguration(DatabaseProvider.POSTGRESQL,
            "192.168.0.1",
            5432,
            "homebook",
            "user",
            "password",
            "my homebook",
            "admin",
            "password",
            true);
        var _configuration = new DataBuilder()
            .ToConfiguration();

        // Act
        var exception = Should.Throw<SetupException>(() => _instance.ProcessAsync(_configuration,
            setupConfiguration,
            _cancellationToken));

        // Assert
        exception.Message.ShouldContain("database provider is not configured");
    }
}
