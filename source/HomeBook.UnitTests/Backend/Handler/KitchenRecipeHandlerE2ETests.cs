using System.Security.Claims;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Search;
using HomeBook.Backend.Data.Sqlite;
using HomeBook.Backend.Data.Sqlite.Extensions;
using HomeBook.Backend.Extensions;
using HomeBook.Backend.Module.Kitchen.Contracts;
using HomeBook.Backend.Module.Kitchen.Handler;
using HomeBook.Backend.Module.Kitchen.Requests;
using HomeBook.Backend.Module.Kitchen.Responses;
using HomeBook.UnitTests.TestCore.Backend;
using HomeBook.UnitTests.TestCore.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeBook.UnitTests.Backend.Handler;

[TestFixture]
public class KitchenRecipeHandlerE2ETests : TestBase
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
    public async Task RunRecipesFullLifecycle_Returns()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        string testUserName = "testuser";
        string testUserPassword = "s3cr3tP@ssw0rd!";

        SearchRegistrationFactory srf = new();
        IConfigurationRoot configuration = CreateTestConfiguration();
        IServiceCollection serviceCollection = CreateTestServiceProvider(configuration);
        IServiceProvider serviceProvider = serviceCollection
            .AddBackendDataSqlite(configuration)
            .AddKeyedSingleton<IDatabaseMigrator, DatabaseMigrator>("SQLITE")
            .AddDependenciesForRuntime(configuration, InstanceStatus.RUNNING)
            .AddBackendModulesForTestEnvironment(configuration, srf)
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
        tables.ShouldContain("RecipeSteps");
        tables.ShouldContain("RecipeIngredients");
        tables.ShouldContain("Recipe2RecipeIngredient");
        tables.ShouldContain("Recipes");

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
        var recipesResult1 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse1 = recipesResult1.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse1.Value.ShouldNotBeNull();
        recipesResponse1.Value.Recipes.Length.ShouldBe(0);

        // create recipes
        var createRecipeResult1 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Gyros-Pita", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult1.ShouldBeOfType<Ok>();
        var createRecipeResult2 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Nana's Italian Roulade",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult2.ShouldBeOfType<Ok>();
        var createRecipeResult3 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pancakes", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult3.ShouldBeOfType<Ok>();
        var createRecipeResult4 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pasta à la Roma", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult4.ShouldBeOfType<Ok>();
        var createRecipeResult5 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Rührei mit Kräutern", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult5.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult2 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse2 = recipesResult2.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse2.Value.ShouldNotBeNull();
        recipesResponse2.Value.Recipes.Length.ShouldBe(5);
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Gyros-Pita"
                                                          && r.NormalizedName == "gyros-pita");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade"
                                                          && r.NormalizedName == "nanas-italian-roulade");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pancakes"
                                                          && r.NormalizedName == "pancakes");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma"
                                                          && r.NormalizedName == "pasta-a-la-roma");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern"
                                                          && r.NormalizedName == "ruhrei-mit-krautern");

        var recipeToDelete = recipesResponse2.Value.Recipes.First(r => r.Name == "Pancakes");
        var deleteRecipeResult = await RecipeHandler.HandleDeleteRecipe(recipeToDelete.Id,
            testuser,
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        deleteRecipeResult.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult3 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse3 = recipesResult3.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse3.Value.ShouldNotBeNull();
        recipesResponse3.Value.Recipes.Length.ShouldBe(4);
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Gyros-Pita"
                                                          && r.NormalizedName == "gyros-pita");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade"
                                                          && r.NormalizedName == "nanas-italian-roulade");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma"
                                                          && r.NormalizedName == "pasta-a-la-roma");
        recipesResponse3.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern"
                                                          && r.NormalizedName == "ruhrei-mit-krautern");

        var recipeToUpdate = recipesResponse3.Value.Recipes.First(r => r.Name == "Gyros-Pita");
        var updateRecipeResult = await RecipeHandler.HandleUpdateRecipeName(recipeToUpdate.Id,
            testuser,
            new UpdateRecipeNameRequest("Gyros Wrap"),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        updateRecipeResult.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult4 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse4 = recipesResult4.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse4.Value.ShouldNotBeNull();
        recipesResponse4.Value.Recipes.Length.ShouldBe(4);
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Gyros Wrap"
                                                          && r.NormalizedName == "gyros-wrap");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade"
                                                          && r.NormalizedName == "nanas-italian-roulade");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma"
                                                          && r.NormalizedName == "pasta-a-la-roma");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern"
                                                          && r.NormalizedName == "ruhrei-mit-krautern");
    }

    [Test]
    public async Task RunRecipesWithStepsAndIngredientsFullLifecycle_Returns()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        string testUserName = "testuser";
        string testUserPassword = "s3cr3tP@ssw0rd!";

        // create configuration and service provider
        SearchRegistrationFactory srf = new();
        IConfigurationRoot configuration = CreateTestConfiguration();
        IServiceCollection serviceCollection = CreateTestServiceProvider(configuration);

        IServiceProvider serviceProvider = serviceCollection
            .AddBackendDataSqlite(configuration)
            .AddKeyedSingleton<IDatabaseMigrator, DatabaseMigrator>("SQLITE")
            .AddDependenciesForRuntime(configuration, InstanceStatus.RUNNING)
            .AddBackendModulesForTestEnvironment(configuration, srf)
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
        tables.ShouldContain("RecipeSteps");
        tables.ShouldContain("RecipeIngredients");
        tables.ShouldContain("Recipe2RecipeIngredient");
        tables.ShouldContain("Recipes");

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
        var recipesResult1 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse1 = recipesResult1.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse1.Value.ShouldNotBeNull();
        recipesResponse1.Value.Recipes.Length.ShouldBe(0);

        // create recipes
        var createRecipeResult1 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Gyros-Pita", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult1.ShouldBeOfType<Ok>();
        var createRecipeResult2 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Nana's Italian Roulade",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult2.ShouldBeOfType<Ok>();
        var createRecipeResult3 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pancakes",
                "Leckere Pfannkuchen mit Schoko-Creme",
                4,
                [
                    new CreateRecipeIngredientRequest("Mehl", 500, "Gramm"),
                    new CreateRecipeIngredientRequest("Salz", 1, "Priese"),
                    new CreateRecipeIngredientRequest("Milch", 1, "Liter"),
                    new CreateRecipeIngredientRequest("Schokolade", 4, "EL"),
                ],
                [
                    new CreateRecipeStepRequest("Mehl mit Salz und Milch mischen.", 0, null),
                    new CreateRecipeStepRequest("Den Teig in eine Pfanne geben und knapp eine Minute anbacken lassen.",
                        1,
                        60),
                    new CreateRecipeStepRequest("Die Schokolade schmelzen lassen", 2, (5 * 60)),
                    new CreateRecipeStepRequest("Die Schokolade über den fertigen Pfannkuchen verteilen", 3, null)
                ],
                8,
                4,
                null,
                2750,
                "Dies ist ein Test-Kommentar",
                "Das habe ich mir selbst ausgedacht"),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult3.ShouldBeOfType<Ok>();
        var createRecipeResult4 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Pasta à la Roma", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult4.ShouldBeOfType<Ok>();
        var createRecipeResult5 = await RecipeHandler.HandleCreateRecipe(testuser,
            new CreateRecipeRequest("Rührei mit Kräutern", null, null, null, null, null, null, null, null, null, null),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        createRecipeResult5.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult2 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse2 = recipesResult2.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse2.Value.ShouldNotBeNull();
        recipesResponse2.Value.Recipes.Length.ShouldBe(5);
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Gyros-Pita"
                                                          && r.NormalizedName == "gyros-pita");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade"
                                                          && r.NormalizedName == "nanas-italian-roulade");

        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pancakes"
                                                          && r.NormalizedName == "pancakes"
                                                          && r.Description == "Leckere Pfannkuchen mit Schoko-Creme"
                                                          && r.DurationWorkingMinutes == 8
                                                          && r.DurationCookingMinutes == 4
                                                          && r.DurationRestingMinutes == null
                                                          && r.CaloriesKcal == 2750
                                                          && r.Servings == 4
                                                          && r.Comments == "Dies ist ein Test-Kommentar"
                                                          && r.Source == "Das habe ich mir selbst ausgedacht");

        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma"
                                                          && r.NormalizedName == "pasta-a-la-roma");
        recipesResponse2.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern"
                                                          && r.NormalizedName == "ruhrei-mit-krautern");

        var recipeToDelete = recipesResponse2.Value.Recipes.First(r => r.Name == "Pancakes");
        var deleteRecipeResult = await RecipeHandler.HandleDeleteRecipe(recipeToDelete.Id,
            testuser,
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        deleteRecipeResult.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult3 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
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

        var recipeToUpdate = recipesResponse3.Value.Recipes.First(r => r.Name == "Gyros-Pita");
        var updateRecipeResult = await RecipeHandler.HandleUpdateRecipeName(recipeToUpdate.Id,
            testuser,
            new UpdateRecipeNameRequest("Gyros Wrap"),
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            cancellationToken);
        updateRecipeResult.ShouldBeOfType<Ok>();

        // get all recipes
        var recipesResult4 = await RecipeHandler.HandleGetRecipes("",
            _loggerFactory.CreateLogger<RecipeHandler>(),
            recipesProvider,
            userProvider,
            cancellationToken);
        var recipesResponse4 = recipesResult4.ShouldBeOfType<Ok<RecipesListResponse>>();
        recipesResponse4.Value.ShouldNotBeNull();
        recipesResponse4.Value.Recipes.Length.ShouldBe(4);
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Gyros Wrap");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.NormalizedName == "gyros-wrap");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Nana's Italian Roulade");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.NormalizedName == "nanas-italian-roulade");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Pasta à la Roma");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.NormalizedName == "pasta-a-la-roma");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.Name == "Rührei mit Kräutern");
        recipesResponse4.Value.Recipes.ShouldContain(r => r.NormalizedName == "ruhrei-mit-krautern");
    }
}
