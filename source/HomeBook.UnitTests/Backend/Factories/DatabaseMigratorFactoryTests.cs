using HomeBook.Backend.Abstractions.Exceptions;
using HomeBook.Backend.Factories;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace HomeBook.UnitTests.Backend.Factories;

[TestFixture]
public class DatabaseMigratorFactoryTests
{
    private IConfiguration _configuration;
    private DatabaseMigratorFactory _instance;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        SetupBasicConfiguration();
        _instance = new DatabaseMigratorFactory(_configuration);
    }

    [Test]
    public void CreateMigrator_WithPostgreSQL_ShouldReturnPostgreSqlMigrator()
    {
        // Act
        var result = _instance.CreateMigrator("POSTGRESQL");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<HomeBook.Backend.Data.PostgreSql.DatabaseMigrator>();
    }

    [Test]
    public void CreateMigrator_WithMySQL_ShouldReturnMySqlMigrator()
    {
        // Act
        var result = _instance.CreateMigrator("MYSQL");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<HomeBook.Backend.Data.Mysql.DatabaseMigrator>();
    }

    [Test]
    public void CreateMigrator_WithInvalidType_ShouldThrowUnsupportedDatabaseException()
    {
        // Act & Assert
        var exception = Should.Throw<UnsupportedDatabaseException>(() => _instance.CreateMigrator("INVALID"));
        exception.Message.ShouldContain("Unsupported database provider: INVALID");
    }

    private void SetupBasicConfiguration()
    {
        _configuration["Database:Host"].Returns("localhost");
        _configuration["Database:Port"].Returns("5432");
        _configuration["Database:InstanceDbName"].Returns("testdb");
        _configuration["Database:Username"].Returns("testuser");
        _configuration["Database:Password"].Returns("testpass");
    }
}
