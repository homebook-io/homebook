using System.Collections.Specialized;
using System.Text.Json;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Core.DataProvider.Validators;
using Homebook.Backend.Core.Setup;
using HomeBook.UnitTests.TestCore.Backend.Core.Setup;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Core.Setup;

[TestFixture]
public class UpdateManagerTests
{
    private CancellationToken _cancellationToken;
    private ILogger<UpdateManager> _logger;
    private IEnumerable<IUpdateMigrator> _availableUpdateMigrators;
    private IApplicationPathProvider _applicationPathProvider;
    private IFileSystemService _fileSystemService;
    private UpdateManager _instance;

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
        _logger = factory.CreateLogger<UpdateManager>();
        _availableUpdateMigrators = Substitute.For<IEnumerable<IUpdateMigrator>>();
        _applicationPathProvider = Substitute.For<IApplicationPathProvider>();
        _fileSystemService = Substitute.For<IFileSystemService>();
        _instance = new UpdateManager(_logger,
            _availableUpdateMigrators,
            _applicationPathProvider,
            _fileSystemService);
    }

    [Test]
    public async Task GetAvailableUpdatesAsync_AfterSetup_Returns()
    {
        // Arrange
        _availableUpdateMigrators = new List<IUpdateMigrator>
        {
            new TestUpdateMigrator("1.0.5", "a short description"),
            new TestUpdateMigrator("1.1.35", "a bigger update"),
            new TestUpdateMigrator("1.0.20", "another description"),
        };
        _instance = new UpdateManager(_logger,
            _availableUpdateMigrators,
            _applicationPathProvider,
            _fileSystemService);
        _fileSystemService
            .FileExists(Path.Combine(_applicationPathProvider.UpdateDirectory, "updates.json"))
            .Returns(false);
        _fileSystemService
            .FileReadAllTextAsync(Path.Combine(_applicationPathProvider.UpdateDirectory, "updates.json"),
                Arg.Any<CancellationToken>())
            .Returns(string.Empty);

        // Act
        Dictionary<Version, string> updates = await _instance.GetAvailableUpdatesAsync(_cancellationToken);

        // Assert
        updates.ShouldNotBeEmpty();
        updates.Count.ShouldBe(3);

        // Verify that updates are returned in ascending version order
        var updateKeys = updates.Keys.ToArray();
        updateKeys[0].ToString().ShouldBe("1.0.5");
        updateKeys[1].ToString().ShouldBe("1.0.20");
        updateKeys[2].ToString().ShouldBe("1.1.35");

        updates.ShouldContain(x => x.Key.ToString() == "1.0.5" && x.Value == "a short description");
        updates.ShouldContain(x => x.Key.ToString() == "1.0.20" && x.Value == "another description");
        updates.ShouldContain(x => x.Key.ToString() == "1.1.35" && x.Value == "a bigger update");
    }

    [Test]
    public async Task GetAvailableUpdatesAsync_NewUpdate_Returns()
    {
        // Arrange
        var versionBeforeUpdate = "1.0.5";
        _availableUpdateMigrators = new List<IUpdateMigrator>
        {
            new TestUpdateMigrator("1.0.5", "a short description"),
            new TestUpdateMigrator("1.1.35", "a bigger update"),
            new TestUpdateMigrator("1.0.20", "another description"),
        };
        _instance = new UpdateManager(_logger,
            _availableUpdateMigrators,
            _applicationPathProvider,
            _fileSystemService);
        _fileSystemService
            .FileExists(Path.Combine(_applicationPathProvider.UpdateDirectory, "updates.json"))
            .Returns(true);
        _fileSystemService
            .FileReadAllTextAsync(Path.Combine(_applicationPathProvider.UpdateDirectory, "updates.json"),
                Arg.Any<CancellationToken>())
            .Returns(JsonSerializer.Serialize(new[] { new Version(versionBeforeUpdate) }));

        // Act
        Dictionary<Version, string> updates = await _instance.GetAvailableUpdatesAsync(_cancellationToken);

        // Assert
        updates.ShouldNotBeEmpty();
        updates.Count.ShouldBe(2);

        // Verify that updates are returned in ascending version order
        var updateKeys = updates.Keys.ToArray();
        updateKeys[0].ToString().ShouldBe("1.0.20");
        updateKeys[1].ToString().ShouldBe("1.1.35");

        updates.ShouldContain(x => x.Key.ToString() == "1.0.20" && x.Value == "another description");
        updates.ShouldContain(x => x.Key.ToString() == "1.1.35" && x.Value == "a bigger update");
    }
}
