using HomeBook.Backend.Handler;
using HomeBook.Backend.OpenApi;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Endpoints;

public static class SetupEndpoints
{
    public static IEndpointRouteBuilder MapSetupEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/setup")
            .WithTags("setup")
            .WithDescription("Endpoints for setup management");

        group.MapGet("/availability", SetupHandler.HandleGetAvailability)
            .WithName("GetAvailability")
            .WithDescription(new Description("returns the status of the setup availability",
                "HTTP 200: Setup is not executed yet and available",
                "HTTP 409: Setup is already executed and not available",
                "HTTP 500: Unknown error while setup checking"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/licenses", SetupHandler.HandleGetLicenses)
            .WithName("GetLicenses")
            .WithDescription(new Description("returns all licenses of the project",
                "HTTP 200: Licenses found",
                "HTTP 500: Unknown error while loading licenses"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<GetLicensesResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/database/configuration", SetupHandler.HandleGetDatabaseCheck)
            .WithName("GetDatabaseCheck")
            .WithDescription(new Description("check if database configuration is available via environment variables",
                "HTTP 200: Database configuration found",
                "HTTP 400: Validation error, e.g. too short password, etc.",
                "HTTP 404: No Database configuration found",
                "HTTP 500: Unknown error while checking Database configuration"))
            .WithOpenApi(operation => new(operation)
            {
                // Summary = "check if database configuration is available via environment variables"
            })
            .Produces<GetDatabaseCheckResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/database/check", SetupHandler.HandleCheckDatabase)
            .WithName("CheckDatabase")
            .WithDescription(new Description("check that the Database is available",
                "HTTP 200: Database is available => returns the detected database provider in uppercases",
                "HTTP 500: Unknown error while database connection check",
                "HTTP 503: Database is not available"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        group.MapPost("/database/migrate", SetupHandler.HandleMigrateDatabase)
            .WithName("MigrateDatabase")
            .WithDescription(new Description("migrate the database schema to the latest version",
                "HTTP 200: Database migration was successful",
                "HTTP 409: Database migration is already executed and not available",
                "HTTP 500: Unknown error while database migration"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/start", SetupHandler.HandleStartSetup)
            .WithName("StartSetup")
            .WithDescription(new Description("start the setup process. it will save the configuration for the setup steps",
                "HTTP 200: Setup started successfully",
                "HTTP 400: Validation error for example with the database configuration, e.g. too short password, etc.",
                "HTTP 422: Licenses not accepted",
                "HTTP 500: Unknown error while starting setup"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapGet("/user", SetupHandler.HandleGetPreConfiguredUser)
            .WithName("GetPreConfiguredUser")
            .WithDescription(new Description("check if a pre-configured user is available via environment variables",
                "HTTP 200: A user was pre-configured",
                "HTTP 404: No pre-configured user found",
                "HTTP 500: Unknown error while loading pre-configured user"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPost("/user", SetupHandler.HandleCreateAdminUser)
            .WithName("CreateAdminUser")
            .WithDescription(new Description("create the admin user for the application",
                "HTTP 201: Admin User created successfully",
                "HTTP 400: Validation error, e.g. too short password, etc.",
                "HTTP 409: User already exists",
                "HTTP 500: Unknown error while creating user"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        group.MapPut("/configuration", SetupHandler.HandleCreateConfiguration)
            .WithName("CreateConfiguration")
            .WithDescription(new Description("create the admin user for the application",
                "HTTP 200: Configuration created successfully",
                "HTTP 400: Invalid configuration, e.g. missing required fields",
                "HTTP 500: Unknown error while updating configuration"))
            .WithOpenApi(operation => new(operation)
            {
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
