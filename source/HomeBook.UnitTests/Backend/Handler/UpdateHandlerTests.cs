using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Handler;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class UpdateHandlerTests
{
    private ILogger<SetupHandler> _logger;
    private ISetupInstanceManager _setupInstanceManager = null!;
    private IHostApplicationLifetime _hostApplicationLifetime = null!;
    private IUpdateProcessor _updateProcessor = null!;

    [SetUp]
    public void SetUpSubstitutes()
    {
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

        _logger = factory.CreateLogger<SetupHandler>();
        _setupInstanceManager = Substitute.For<ISetupInstanceManager>();
        _hostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();
        _updateProcessor = Substitute.For<IUpdateProcessor>();
    }

    [Test]
    public async Task HandleStartUpdate_NoSetupExecuted_Returns()
    {
        // Arrange
        _setupInstanceManager
            .IsHomebookInstanceCreated()
            .Returns(false);

        // Act
        var result = await UpdateHandler.HandleStartUpdate(_logger,
            _setupInstanceManager,
            _hostApplicationLifetime,
            _updateProcessor,
            CancellationToken.None);

        // Assert
        var response = result.ShouldBeOfType<Conflict>();
        response.ShouldNotBeNull();
    }

    [Test]
    public async Task HandleStartUpdate_NoUpdateAvailable_Returns()
    {
        // Arrange
        _setupInstanceManager
            .IsHomebookInstanceCreated()
            .Returns(true);
        _setupInstanceManager
            .IsUpdateRequiredAsync()
            .Returns(false);

        // Act
        var result = await UpdateHandler.HandleStartUpdate(_logger,
            _setupInstanceManager,
            _hostApplicationLifetime,
            _updateProcessor,
            CancellationToken.None);

        // Assert
        var response = result.ShouldBeOfType<Ok>();
        response.ShouldNotBeNull();
    }
}
