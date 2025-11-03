using System.Security.Claims;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Data.Sqlite;
using HomeBook.Backend.Data.Sqlite.Extensions;
using HomeBook.Backend.DTOs.Requests.Finances;
using HomeBook.Backend.Extensions;
using HomeBook.Backend.Handler;
using HomeBook.Backend.Requests;
using HomeBook.Backend.Responses;
using HomeBook.UnitTests.TestCore.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class FinanceSavingGoalHandlerE2ETests
{
    private ILoggerFactory _loggerFactory;
    private SqliteConnection _keepAlive = null!;

    [SetUp]
    public void SetUpSubstitutes()
    {
        // create logger
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

        // create sqlite in memory
        var connectionString = ConnectionStringBuilder.BuildInMemory();

        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
    }

    [TearDown]
    public void TearDown()
    {
        // delete sqlite in memory
        _keepAlive.Close();

        // dispose logger
        _loggerFactory.Dispose();
    }

    [Test]
    public async Task RunFullLifecycle_Returns()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        string testUserName = "testuser";
        string testUserPassword = "s3cr3tP@ssw0rd!";

        // create configuration and service provider
        IConfigurationRoot configuration = ConfigurationHelper.CreateConfigurationRoot(new Dictionary<string, string>
        {
            ["Environment"] = "UnitTests",
            ["Database:UseInMemory"] = "true",
            ["Database:Provider"] = "SQLITE"
        });
        IServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton(configuration)
            .AddBackendDataSqlite(configuration)
            .AddKeyedSingleton<IDatabaseMigrator, DatabaseMigrator>("SQLITE")
            .AddDependenciesForRuntime(configuration, InstanceStatus.SETUP)
            .BuildServiceProvider();

        // apply migrations
        var databaseMigrator = serviceProvider.GetKeyedService<IDatabaseMigrator>("SQLITE");
        await databaseMigrator.MigrateAsync(cancellationToken);

        // verify that migrations were applied
        var tables = SqliteHelper.GetAllTableNames(_keepAlive)
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
        tables.ShouldContain("__EFMigrationsHistory");
        tables.ShouldContain("Users");
        tables.ShouldContain("Configurations");
        tables.ShouldContain("UserPreferences");
        tables.ShouldContain("SavingGoals");

        // create user
        IUserProvider userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        Guid testUserId = await userProvider.CreateUserAsync(testUserName,
            testUserPassword,
            cancellationToken);
        ClaimsPrincipal testuser = UserHelper.CreateTestUser(testUserId,
            testUserName);

        // create instances for tests
        ISavingGoalsProvider savingGoalsProvider = serviceProvider.GetRequiredService<ISavingGoalsProvider>();

        // Act & Assert

        // create saving goal
        var createRequest = new CreateSavingGoalRequest("Test Saving Goal",
            "#ff0000",
            12_000,
            8_500,
            null);
        var createSavingGoalResult = await FinanceSavingGoalHandler.HandleCreateSavingGoal(testuser,
            createRequest,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        createSavingGoalResult.ShouldBeOfType<Ok>();

        // get all after creation
        var savingGoalsResult1 = await FinanceSavingGoalHandler.HandleGetSavingGoals(testuser,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        var savingGoalsResponse1 = savingGoalsResult1.ShouldBeOfType<Ok<GetFinanceSavingGoalsResponse>>();
        savingGoalsResponse1.Value.ShouldNotBeNull();
        savingGoalsResponse1.Value.SavingGoals.Length.ShouldBe(1);
        Guid createdSavingGoalId = savingGoalsResponse1.Value.SavingGoals[0].Id;
        savingGoalsResponse1.Value.SavingGoals[0].Color.ShouldBe("#ff0000");
        savingGoalsResponse1.Value.SavingGoals[0].Name.ShouldBe("Test Saving Goal");
        savingGoalsResponse1.Value.SavingGoals[0].TargetAmount.ShouldBe(12_000);
        savingGoalsResponse1.Value.SavingGoals[0].CurrentAmount.ShouldBe(8_500);

        // // update saving goal
        var updateRequest = new CreateSavingGoalRequest("Updated Saving Goal",
            "#00ff00",
            12_000,
            9_000,
            null);
        var updateSavingGoalResult = await FinanceSavingGoalHandler.HandleUpdateSavingGoal(createdSavingGoalId,
            testuser,
            updateRequest,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        updateSavingGoalResult.ShouldBeOfType<Ok>();

        // get all after update
        var savingGoalsResult2 = await FinanceSavingGoalHandler.HandleGetSavingGoals(testuser,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        var savingGoalsResponse2 = savingGoalsResult2.ShouldBeOfType<Ok<GetFinanceSavingGoalsResponse>>();
        savingGoalsResponse2.Value.ShouldNotBeNull();
        savingGoalsResponse2.Value.SavingGoals.Length.ShouldBe(1);
        savingGoalsResponse2.Value.SavingGoals[0].Color.ShouldBe("#00ff00");
        savingGoalsResponse2.Value.SavingGoals[0].Name.ShouldBe("Updated Saving Goal");
        savingGoalsResponse2.Value.SavingGoals[0].TargetAmount.ShouldBe(12_000);
        savingGoalsResponse2.Value.SavingGoals[0].CurrentAmount.ShouldBe(9_000);

        // // delete saving goal
        var deleteSavingGoalResult = await FinanceSavingGoalHandler.HandleDeleteSavingGoal(createdSavingGoalId,
            testuser,
            updateRequest,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        deleteSavingGoalResult.ShouldBeOfType<Ok>();

        // get all after delete
        var savingGoalsResult3 = await FinanceSavingGoalHandler.HandleGetSavingGoals(testuser,
            _loggerFactory.CreateLogger<FinanceSavingGoalHandler>(),
            savingGoalsProvider,
            cancellationToken);
        var savingGoalsResponse3 = savingGoalsResult3.ShouldBeOfType<Ok<GetFinanceSavingGoalsResponse>>();
        savingGoalsResponse3.Value.ShouldNotBeNull();
        savingGoalsResponse3.Value.SavingGoals.Length.ShouldBe(0);
    }
}
