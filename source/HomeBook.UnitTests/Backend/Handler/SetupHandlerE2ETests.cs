using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HomeBook.Backend.Core.HashProvider;
using HomeBook.Backend.Core.Licenses;
using Homebook.Backend.Core.Setup;
using Homebook.Backend.Core.Setup.Factories;
using Homebook.Backend.Core.Setup.Provider;
using Homebook.Backend.Core.Setup.Validators;
using HomeBook.Backend.Factories;
using HomeBook.Backend.Handler;
using HomeBook.Backend.Provider;
using HomeBook.Backend.Requests;
using HomeBook.UnitTests.TestCore;
using HomeBook.UnitTests.TestCore.Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class SetupHandlerE2ETests
{
    private ILoggerFactory _loggerFactory;

    [SetUp]
    public void SetUpSubstitutes()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                })
                .SetMinimumLevel(LogLevel.Debug);
        });
    }

    [TearDown]
    public void TearDown()
    {
        _loggerFactory.Dispose();
    }

    [Test]
    public async Task HandleStartSetup_RunFullSetupWithUpdates_Returns()
    {
        // Arrange
        var request = new StartSetupRequest(true,
            "POSTGRESQL",
            "localhost",
            5432,
            "homebook-test",
            "root",
            "s3cr3t-p1ssw0rd",
            "testuser",
            "s3cr3t-p1ssw0rd",
            "Test Homebook",
            "DE");
        var configuration = new DataBuilder()
            // .Add("", "")
            .ToConfiguration();

        var applicationPathProvider = new TestFileService();
        var fileSystemService = new TestFileService();
        var runtimeConfigurationProvider = new RuntimeConfigurationProvider(
            _loggerFactory.CreateLogger<RuntimeConfigurationProvider>(),
            applicationPathProvider,
            fileSystemService);
        var setupInstanceManager = new SetupInstanceManager(
            _loggerFactory.CreateLogger<SetupInstanceManager>(),
            configuration,
            fileSystemService,
            applicationPathProvider);
        var licenseProvider = new LicenseProvider(
            _loggerFactory.CreateLogger<LicenseProvider>(),
            configuration,
            fileSystemService,
            applicationPathProvider);
        var setupConfigurationProvider = new SetupConfigurationProvider(
            _loggerFactory.CreateLogger<SetupConfigurationProvider>(),
            new EnvironmentValidator());
        var hostApplicationLifetime = new TestHostApplicationLifetime();
        var databaseMigratorFactory = new DatabaseMigratorFactory(configuration);
        var hashProviderFactory = new HashProviderFactory();
        var setupProcessorFactory = new SetupProcessorFactory(
            databaseMigratorFactory,
            hashProviderFactory,
            _loggerFactory,
            configuration,
            fileSystemService,
            applicationPathProvider);

        // Act
        var result = await SetupHandler.HandleStartSetup(request,
            _loggerFactory.CreateLogger<SetupHandler>(),
            new SetupConfigurationValidator(),
            runtimeConfigurationProvider,
            setupInstanceManager,
            licenseProvider,
            setupConfigurationProvider,
            configuration,
            hostApplicationLifetime,
            setupProcessorFactory,
            CancellationToken.None);

        // Assert
        var response = result.ShouldBeOfType<Ok>();
        response.ShouldNotBeNull();
    }
}
