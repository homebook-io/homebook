using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.DataProvider.UserManagement;
using HomeBook.Backend.Core.DataProvider.Validators;
using HomeBook.Backend.Core.HashProvider;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Core.DataProvider.UserManagement;

[TestFixture]
public class UserProviderTests
{
    private CancellationToken _cancellationToken;
    private ILogger<UserProvider> _logger;
    private IUserRepository _userRepository;
    private UserProvider _instance;

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
        _logger = factory.CreateLogger<UserProvider>();
        _userRepository = Substitute.For<IUserRepository>();
        _instance = new UserProvider(
            // _logger,
            _userRepository,
            new HashProviderFactory(),
            new UserValidator());
    }

    [Test]
    public async Task CreateUserAsync_WithValidUser_CallCreate()
    {
        // Arrange
        string username = "testuser";
        string password = "A-s3cr3t-P@ssw0rd!";
        _userRepository.ContainsUserAsync(username, _cancellationToken)
            .Returns(false);

        // Act
        await _instance.CreateUserAsync(username, password, _cancellationToken);

        // Assert
        await _userRepository.Received(1)
            .CreateUserAsync(
                Arg.Is<User>(u => u.Username == username &&
                                  !string.IsNullOrEmpty(u.PasswordHash) &&
                                  !string.IsNullOrEmpty(u.PasswordHashType)),
                _cancellationToken);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ContainsUserAsync_ShouldReturnExpectedResult(bool containsUser)
    {
        // Arrange
        string username = "testuser";
        _userRepository.ContainsUserAsync(username, _cancellationToken)
            .Returns(containsUser);

        // Act
        var result = await _instance.ContainsUserAsync(username, _cancellationToken);

        // Assert
        result.ShouldBe(containsUser);
        await _userRepository.Received(1).ContainsUserAsync(username, _cancellationToken);
    }
}
