using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.HashProvider;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Handler;
using HomeBook.Backend.Requests;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class SystemHandlerTests
{
    private IConfiguration _configuration = null!;
    private IUserRepository _userRepository = null!;
    private IHashProviderFactory _hashProviderFactory = null!;
    private IJwtService _jwtService = null!;
    private HttpContext _httpContext = null!;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        _userRepository = Substitute.For<IUserRepository>();
        _jwtService = Substitute.For<IJwtService>();
        _httpContext = Substitute.For<HttpContext>();
        _hashProviderFactory = new HashProviderFactory();
        _cancellationToken = CancellationToken.None;
    }

    #region HandleGetSystemInfo Tests

    [Test]
    public void HandleGetSystemInfo_ReturnsOk_WithSystemInformation()
    {
        // Arrange
        _configuration["Version"].Returns("1.0.0");
        _configuration["Database:Provider"].Returns("PostgreSQL");

        // Act
        IResult result = SystemHandler.HandleGetSystemInfo(_configuration, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<GetSystemInfoResponse>>();
        var okResult = (Ok<GetSystemInfoResponse>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.AppVersion.ShouldBe("1.0.0");
        okResult.Value.DatabaseProvider.ShouldBe("PostgreSQL");
        okResult.Value.DeploymentType.ShouldBe("Docker");
        okResult.Value.DotnetRuntimeVersion.ShouldBe(Environment.Version.ToString());
    }

    [Test]
    public void HandleGetSystemInfo_ReturnsOk_WithDefaultValues_WhenConfigurationIsNull()
    {
        // Arrange
        _configuration["Version"].Returns((string?)null);
        _configuration["Database:Provider"].Returns((string?)null);

        // Act
        IResult result = SystemHandler.HandleGetSystemInfo(_configuration, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<GetSystemInfoResponse>>();
        var okResult = (Ok<GetSystemInfoResponse>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.AppVersion.ShouldBe("Unknown");
        okResult.Value.DatabaseProvider.ShouldBe("Unknown");
    }

    [Test]
    public void HandleGetSystemInfo_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        _configuration["Version"].ThrowsForAnyArgs<Exception>();

        // Act
        IResult result = SystemHandler.HandleGetSystemInfo(_configuration, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleGetUsers Tests

    [Test]
    public async Task HandleGetUsers_ReturnsOk_WithPaginatedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() {
                Id = Guid.NewGuid(),
                Username = "user1",
                PasswordHash = "hash1",
                PasswordHashType = "Test",
                Created = DateTime.UtcNow,
                IsAdmin = true
            },
            new() {
                Id = Guid.NewGuid(),
                Username = "user2",
                PasswordHash = "hash2",
                PasswordHashType = "Test",
                Created = DateTime.UtcNow,
                IsAdmin = false
            },
            new() {
                Id = Guid.NewGuid(),
                Username = "user3",
                PasswordHash = "hash3",
                PasswordHashType = "Test",
                Created = DateTime.UtcNow,
                IsAdmin = false
            }
        };

        _userRepository.GetAllAsync(_cancellationToken).Returns(users);

        // Act
        IResult result = await SystemHandler.HandleGetUsers(_userRepository, 1, 2, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<GetUsersResponse>>();
        var okResult = (Ok<GetUsersResponse>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.TotalCount.ShouldBe(3);
        okResult.Value.Page.ShouldBe(1);
        okResult.Value.PageSize.ShouldBe(2);
        okResult.Value.TotalPages.ShouldBe(2);
        okResult.Value.Users.Count().ShouldBe(2);
    }

    [Test]
    public async Task HandleGetUsers_NormalizesPageParameters_WhenInvalid()
    {
        // Arrange
        var users = new List<User>();
        _userRepository.GetAllAsync(_cancellationToken).Returns(users);

        // Act
        IResult result = await SystemHandler.HandleGetUsers(_userRepository, -1, 0, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<GetUsersResponse>>();
        var okResult = (Ok<GetUsersResponse>)result;
        okResult.Value!.Page.ShouldBe(1);
        okResult.Value.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task HandleGetUsers_LimitsPageSize_WhenTooLarge()
    {
        // Arrange
        var users = new List<User>();
        _userRepository.GetAllAsync(_cancellationToken).Returns(users);

        // Act
        IResult result = await SystemHandler.HandleGetUsers(_userRepository, 1, 200, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<GetUsersResponse>>();
        var okResult = (Ok<GetUsersResponse>)result;
        okResult.Value!.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task HandleGetUsers_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        _userRepository.GetAllAsync(_cancellationToken).ThrowsAsync<Exception>();

        // Act
        IResult result = await SystemHandler.HandleGetUsers(_userRepository, 1, 10, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleCreateUser Tests

    [Test]
    public async Task HandleCreateUser_ReturnsOk_WithUserId_WhenUserCreatedSuccessfully()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "password123", true);
        var userId = Guid.NewGuid();
        var createdUser = new User {
            Id = userId,
            Username = "testuser",
            PasswordHash = "hashedpassword",
            PasswordHashType = "TestHashProvider"
        };

        _userRepository.ContainsUserAsync("testuser", _cancellationToken).Returns(false);
        _userRepository.CreateUserAsync(Arg.Any<User>(), _cancellationToken).Returns(createdUser);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<CreateUserResponse>>();
        var okResult = (Ok<CreateUserResponse>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.UserId.ShouldBe(userId);
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenUsernameIsEmpty()
    {
        // Arrange
        var request = new CreateUserRequest("", "password123", false);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Username is required");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "", false);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Password is required");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenUsernameTooShort()
    {
        // Arrange
        var request = new CreateUserRequest("test", "password123", false);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Username must be between 5 and 20 characters");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenUsernameTooLong()
    {
        // Arrange
        var request = new CreateUserRequest("verylongusernamethatistoolong", "password123", false);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Username must be between 5 and 20 characters");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenPasswordTooShort()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "short", false);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Password must be at least 8 characters long");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsBadRequest_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "password123", false);
        _userRepository.ContainsUserAsync("testuser", _cancellationToken).Returns(true);

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Username already exists");
    }

    [Test]
    public async Task HandleCreateUser_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "password123", false);
        _userRepository.ContainsUserAsync("testuser", _cancellationToken).ThrowsAsync<Exception>();

        // Act
        IResult result = await SystemHandler.HandleCreateUser(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleDeleteUser Tests

    [Test]
    public async Task HandleDeleteUser_ReturnsOk_WhenUserDeletedSuccessfully()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var userToDeleteId = Guid.NewGuid();
        var request = new DeleteUserRequest(userToDeleteId);
        var userToDelete = new User {
            Id = userToDeleteId,
            Username = "usertodelete",
            PasswordHash = "hash",
            PasswordHashType = "Test"
        };
        var updatedUser = new User {
            Id = userToDeleteId,
            Username = "usertodelete",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = DateTime.UtcNow
        };

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(userToDeleteId, _cancellationToken).Returns(userToDelete);
        _userRepository.DeleteAsync(userToDeleteId, _cancellationToken).Returns(true);

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.ShouldBe("User deleted successfully");
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsUnauthorized_WhenNoAuthorizationHeader()
    {
        // Arrange
        var request = new DeleteUserRequest(Guid.NewGuid());
        SetupHttpContextWithoutAuthHeader();

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var request = new DeleteUserRequest(Guid.NewGuid());
        SetupHttpContextWithBearerToken("invalid-token");
        _jwtService.GetUserIdFromToken("invalid-token").Returns((Guid?)null);

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsBadRequest_WhenUserTriesToDeleteThemselves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new DeleteUserRequest(userId);

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(userId);

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("You cannot delete your own account");
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsNotFound_WhenUserToDeleteDoesNotExist()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var userToDeleteId = Guid.NewGuid();
        var request = new DeleteUserRequest(userToDeleteId);

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(userToDeleteId, _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe("User not found");
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsProblem_WhenUpdateFails()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var userToDeleteId = Guid.NewGuid();
        var request = new DeleteUserRequest(userToDeleteId);
        var userToDelete = new User {
            Id = userToDeleteId,
            Username = "usertodelete",
            PasswordHash = "hash",
            PasswordHashType = "Test"
        };

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(userToDeleteId, _cancellationToken).Returns(userToDelete);
        _userRepository.UpdateAsync(userToDeleteId, Arg.Any<Action<User>>(), _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Test]
    public async Task HandleDeleteUser_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        var request = new DeleteUserRequest(Guid.NewGuid());
        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").ThrowsForAnyArgs<Exception>();

        // Act
        IResult result = await SystemHandler.HandleDeleteUser(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleUpdatePassword Tests

    [Test]
    public async Task HandleUpdatePassword_ReturnsOk_WhenPasswordUpdatedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdatePasswordRequest(userId, "newpassword123");
        var user = new User {
            Id = userId,
            Username = "testuser",
            PasswordHash = "oldhash",
            PasswordHashType = "Test"
        };
        var updatedUser = new User {
            Id = userId,
            Username = "testuser",
            PasswordHash = "newhash",
            PasswordHashType = "TestHashProvider"
        };

        _userRepository.GetUserByIdAsync(userId, _cancellationToken).Returns(user);
        _userRepository.UpdateAsync(userId, Arg.Any<Action<User>>(), _cancellationToken).Returns(updatedUser);

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.ShouldBe("Password updated successfully");
    }

    [Test]
    public async Task HandleUpdatePassword_ReturnsBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new UpdatePasswordRequest(Guid.NewGuid(), "");

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Password is required");
    }

    [Test]
    public async Task HandleUpdatePassword_ReturnsBadRequest_WhenPasswordTooShort()
    {
        // Arrange
        var request = new UpdatePasswordRequest(Guid.NewGuid(), "short");

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Password must be at least 8 characters long");
    }

    [Test]
    public async Task HandleUpdatePassword_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdatePasswordRequest(userId, "newpassword123");
        _userRepository.GetUserByIdAsync(userId, _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe("User not found");
    }

    [Test]
    public async Task HandleUpdatePassword_ReturnsProblem_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdatePasswordRequest(userId, "newpassword123");
        var user = new User {
            Id = userId,
            Username = "testuser",
            PasswordHash = "oldhash",
            PasswordHashType = "Test"
        };

        _userRepository.GetUserByIdAsync(userId, _cancellationToken).Returns(user);
        _userRepository.UpdateAsync(userId, Arg.Any<Action<User>>(), _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Test]
    public async Task HandleUpdatePassword_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        var request = new UpdatePasswordRequest(Guid.NewGuid(), "newpassword123");
        _userRepository.GetUserByIdAsync(Arg.Any<Guid>(), _cancellationToken).ThrowsAsync<Exception>();

        // Act
        IResult result = await SystemHandler.HandleUpdatePassword(_userRepository, _hashProviderFactory, request, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleUpdateUserAdmin Tests

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsOk_WhenAdminStatusUpdatedSuccessfully()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var request = new UpdateUserAdminRequest(targetUserId, true);
        var user = new User {
            Id = targetUserId,
            Username = "targetuser",
            PasswordHash = "hash",
            PasswordHashType = "Test"
        };
        var updatedUser = new User {
            Id = targetUserId,
            Username = "targetuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            IsAdmin = true
        };

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(user);
        _userRepository.UpdateAsync(targetUserId, Arg.Any<Action<User>>(), _cancellationToken).Returns(updatedUser);

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.ShouldBe("User admin status updated successfully to True");
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsUnauthorized_WhenNoAuthorizationHeader()
    {
        // Arrange
        var request = new UpdateUserAdminRequest(Guid.NewGuid(), true);
        SetupHttpContextWithoutAuthHeader();

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var request = new UpdateUserAdminRequest(Guid.NewGuid(), true);
        SetupHttpContextWithBearerToken("invalid-token");
        _jwtService.GetUserIdFromToken("invalid-token").Returns((Guid?)null);

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsBadRequest_WhenUserTriesToChangeOwnStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserAdminRequest(userId, false);

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(userId);

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("You cannot change your own admin status");
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsNotFound_WhenTargetUserDoesNotExist()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var request = new UpdateUserAdminRequest(targetUserId, true);

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe("User not found");
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsProblem_WhenUpdateFails()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var request = new UpdateUserAdminRequest(targetUserId, true);
        var user = new User {
            Id = targetUserId,
            Username = "targetuser",
            PasswordHash = "hash",
            PasswordHashType = "Test"
        };

        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(user);
        _userRepository.UpdateAsync(targetUserId, Arg.Any<Action<User>>(), _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Test]
    public async Task HandleUpdateUserAdmin_ReturnsProblem_WhenExceptionOccurs()
    {
        // Arrange
        var request = new UpdateUserAdminRequest(Guid.NewGuid(), true);
        SetupHttpContextWithBearerToken("valid-token");
        _jwtService.GetUserIdFromToken("valid-token").ThrowsForAnyArgs<Exception>();

        // Act
        IResult result = await SystemHandler.HandleUpdateUserAdmin(_userRepository, _jwtService, request, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    #endregion

    #region HandleEnableUser Tests

    [Test]
    public async Task HandleEnableUser_WithValidRequest_ReturnsOk()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";

        User disabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = DateTime.UtcNow
        };
        User enabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = null
        };

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(disabledUser);
        _userRepository.UpdateAsync(targetUserId, Arg.Any<Action<User>>(), _cancellationToken).Returns(enabledUser);

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.ShouldBe("User enabled successfully");
    }

    [Test]
    public async Task HandleEnableUser_WithoutAuthHeader_ReturnsUnauthorized()
    {
        // Arrange
        Guid targetUserId = Guid.NewGuid();
        _httpContext.Request.Headers.Returns(new HeaderDictionary());

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleEnableUser_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        string token = "invalid-token";
        Guid targetUserId = Guid.NewGuid();

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns((Guid?)null);

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleEnableUser_WhenUserTriesToEnableSelf_ReturnsBadRequest()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        string token = "valid-token";

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(userId);

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, userId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("You cannot enable your own account");
    }

    [Test]
    public async Task HandleEnableUser_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe("User not found");
    }

    [Test]
    public async Task HandleEnableUser_WhenUserAlreadyEnabled_ReturnsBadRequest()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";
        User enabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = null
        };

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(enabledUser);

        // Act
        IResult result = await SystemHandler.HandleEnableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("User is already enabled");
    }

    #endregion

    #region HandleDisableUser Tests

    [Test]
    public async Task HandleDisableUser_WithValidRequest_ReturnsOk()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";

        User enabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = null
        };
        User disabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = DateTime.UtcNow
        };

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(enabledUser);
        _userRepository.UpdateAsync(targetUserId, Arg.Any<Action<User>>(), _cancellationToken).Returns(disabledUser);

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.ShouldBe("User disabled successfully");
    }

    [Test]
    public async Task HandleDisableUser_WithoutAuthHeader_ReturnsUnauthorized()
    {
        // Arrange
        Guid targetUserId = Guid.NewGuid();
        _httpContext.Request.Headers.Returns(new HeaderDictionary());

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleDisableUser_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        string token = "invalid-token";
        Guid targetUserId = Guid.NewGuid();

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns((Guid?)null);

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<UnauthorizedHttpResult>();
    }

    [Test]
    public async Task HandleDisableUser_WhenUserTriesToDisableSelf_ReturnsBadRequest()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        string token = "valid-token";

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(userId);

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, userId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("You cannot disable your own account");
    }

    [Test]
    public async Task HandleDisableUser_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns((User?)null);

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe("User not found");
    }

    [Test]
    public async Task HandleDisableUser_WhenUserAlreadyDisabled_ReturnsBadRequest()
    {
        // Arrange
        Guid currentUserId = Guid.NewGuid();
        Guid targetUserId = Guid.NewGuid();
        string token = "valid-token";
        User disabledUser = new()
        {
            Id = targetUserId,
            Username = "testuser",
            PasswordHash = "hash",
            PasswordHashType = "Test",
            Disabled = DateTime.UtcNow
        };

        SetupHttpContext(token);
        _jwtService.GetUserIdFromToken(token).Returns(currentUserId);
        _userRepository.GetUserByIdAsync(targetUserId, _cancellationToken).Returns(disabledUser);

        // Act
        IResult result = await SystemHandler.HandleDisableUser(_userRepository, _jwtService, targetUserId, _httpContext, _cancellationToken);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("User is already disabled");
    }

    #endregion

    #region Helper Methods

    private void SetupHttpContextWithBearerToken(string token)
    {
        var headers = Substitute.For<IHeaderDictionary>();
        var authHeaderValue = new StringValues($"Bearer {token}");
        headers["Authorization"].Returns(authHeaderValue);

        var request = Substitute.For<HttpRequest>();
        request.Headers.Returns(headers);
        request.Headers["Authorization"].Returns(authHeaderValue);
        request.Headers.Authorization.Returns(authHeaderValue);

        _httpContext.Request.Returns(request);
    }

    private void SetupHttpContextWithoutAuthHeader()
    {
        var headers = Substitute.For<IHeaderDictionary>();
        var emptyHeaderValue = new StringValues();
        headers["Authorization"].Returns(emptyHeaderValue);

        var request = Substitute.For<HttpRequest>();
        request.Headers.Returns(headers);
        request.Headers["Authorization"].Returns(emptyHeaderValue);
        request.Headers.Authorization.Returns(emptyHeaderValue);

        _httpContext.Request.Returns(request);
    }

    private void SetupHttpContext(string token)
    {
        HeaderDictionary headers = new()
        {
            { "Authorization", $"Bearer {token}" }
        };
        _httpContext.Request.Headers.Returns(headers);
    }

    #endregion

    #region Test Helper Classes

    private class TestHashProvider : IHashProvider
    {
        public string AlgorithmName => "Test";
        public string Hash(string input) => "hashed" + input;
        public bool Verify(string input, string hash) => hash == "hashed" + input;
    }

    #endregion
}
