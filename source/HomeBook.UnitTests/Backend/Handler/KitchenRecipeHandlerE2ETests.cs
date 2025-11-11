using System.Security.Claims;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Kitchen.Contracts;
using HomeBook.Backend.Data.Sqlite;
using HomeBook.Backend.Data.Sqlite.Extensions;
using HomeBook.Backend.DTOs.Requests.Kitchen;
using HomeBook.Backend.DTOs.Responses.Kitchen;
using HomeBook.Backend.Extensions;
using HomeBook.Backend.Handler;
using HomeBook.UnitTests.TestCore.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class KitchenRecipeHandlerE2ETests
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
            .AddDependenciesForRuntime(configuration, InstanceStatus.RUNNING)
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
        tables.ShouldContain("Recipes");
        tables.ShouldContain("Recipes");
        tables.ShouldContain("Ingredients");

        // create user
        IUserProvider userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        Guid testUserId = await userProvider.CreateUserAsync(testUserName,
            testUserPassword,
            cancellationToken);
        ClaimsPrincipal testuser = UserHelper.CreateTestUser(testUserId,
            testUserName);

        // create instances for tests
        IRecipesProvider recipesProvider = serviceProvider.GetRequiredService<IRecipesProvider>();

        // Act & Assert

        // get all recipes
        var recipesResult1 = await KitchenRecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        var recipesResponse1 = recipesResult1.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse1.Value.ShouldNotBeNull();
        recipesResponse1.Value.Recipes.Length.ShouldBe(0);

        // create recipes
        var createRecipeResult1 = await KitchenRecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Gyros-Pita"),
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult1.ShouldBeOfType<Ok>();
        var createRecipeResult2 = await KitchenRecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Nana's Italian Roulade"),
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult2.ShouldBeOfType<Ok>();
        var createRecipeResult3 = await KitchenRecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pancakes"),
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult3.ShouldBeOfType<Ok>();
        var createRecipeResult4 = await KitchenRecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pasta à la Roma"),
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult4.ShouldBeOfType<Ok>();
        var createRecipeResult5 = await KitchenRecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Rührei mit Kräutern"),
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult5.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult2 = await KitchenRecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        var recipesResponse2 = recipesResult2.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse2.Value.ShouldNotBeNull();
        recipesResponse2.Value.Recipes.Length.ShouldBe(5);
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Gyros-Pita");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.NormalizedName == "gyros-pita");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.NormalizedName == "nanas-italian-roulade");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pancakes");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.NormalizedName == "pancakes");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.NormalizedName == "pasta-a-la-roma");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.NormalizedName == "ruhrei-mit-krautern");

        var recipeToDelete = recipesResponse2.Value.Recipes.First(r => r.Name == "Pancakes");
        var deleteRecipeResult = await KitchenRecipeHandler.HandleDeleteRecipe(recipeToDelete.Id,
            testuser,
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        deleteRecipeResult.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult3 = await KitchenRecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<KitchenRecipeHandler>(),
            recipesProvider,
            cancellationToken);
        var recipesResponse3 = recipesResult3.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse3.Value.ShouldNotBeNull();
        recipesResponse3.Value.Recipes.Length.ShouldBe(4);
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Gyros-Pita");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.NormalizedName == "gyros-pita");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.NormalizedName == "nanas-italian-roulade");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.NormalizedName == "pasta-a-la-roma");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.NormalizedName == "ruhrei-mit-krautern");
    }
}
