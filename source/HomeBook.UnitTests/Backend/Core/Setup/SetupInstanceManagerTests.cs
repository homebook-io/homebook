using HomeBook.Backend.Abstractions;
using Homebook.Backend.Core.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HomeBook.UnitTests.Backend.Core.Setup;

[TestFixture]
public class SetupInstanceManagerTests
{
    private CancellationToken _cancellationToken;
    private ILogger<SetupInstanceManager> _logger;
    private IConfiguration _configuration;
    private IFileSystemService _fileSystemService;
    private IApplicationPathProvider _applicationPathProvider;
    private SetupInstanceManager _instance;

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
        _logger = factory.CreateLogger<SetupInstanceManager>();
        _configuration = Substitute.For<IConfiguration>();
        _fileSystemService = Substitute.For<IFileSystemService>();
        _applicationPathProvider = Substitute.For<IApplicationPathProvider>();
        _instance = new SetupInstanceManager(_logger, _configuration, _fileSystemService, _applicationPathProvider);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithNullVersions_ReturnNoUpdate()
    {
        // Arrange
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns((string)null!);
        _configuration.GetSection("Version").Returns(configSection);

        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns((string)null!);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithAppVersionAndNullInstanceVersion_ReturnNoUpdate()
    {
        // Arrange
        const string expectedVersion = "1.2.3";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns(expectedVersion);
        _configuration.GetSection("Version").Returns(configSection);

        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns((string)null!);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithNullAppVersionAndInstanceVersion_ReturnNoUpdate()
    {
        // Arrange
        const string expectedVersion = "1.2.3";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns((string)null!);
        _configuration.GetSection("Version").Returns(configSection);

        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedVersion);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithSameVersions_ReturnNoUpdate()
    {
        // Arrange
        const string expectedVersion = "1.2.3";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns(expectedVersion);
        _configuration.GetSection("Version").Returns(configSection);

        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedVersion);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithLowerAppVersion_ReturnNoUpdate()
    {
        // Arrange
        const string appVersion = "1.0.0";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns(appVersion);
        _configuration.GetSection("Version").Returns(configSection);

        const string instanceVersion = "1.1.0";
        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(instanceVersion);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithHigherAppVersion_ReturnUpdateAvailable()
    {
        // Arrange
        const string appVersion = "1.1.0";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns(appVersion);
        _configuration.GetSection("Version").Returns(configSection);

        const string instanceVersion = "1.0.0";
        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(instanceVersion);

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(true);
    }

    [Test]
    public async Task IsUpdateRequiredAsync_WithFileReadException_ReturnNoUpdate()
    {
        // Arrange
        const string appVersion = "1.1.0";
        var configSection = Substitute.For<IConfigurationSection>();
        configSection.Value.Returns(appVersion);
        _configuration.GetSection("Version").Returns(configSection);

        _fileSystemService.FileReadAllTextAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Throws(new FileNotFoundException("Instance file not found"));

        // Act
        var result = await _instance.IsUpdateRequiredAsync(_cancellationToken);

        // Assert
        result.ShouldBe(false);
    }
}
