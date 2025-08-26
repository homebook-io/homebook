using FluentValidation;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Abstractions.Setup;
using HomeBook.Backend.Core.Models;
using HomeBook.Backend.Requests;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public class SetupHandler
{
    /// <summary>
    /// checks if the setup is available and no setup instance is created yet.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="setupInstanceManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetAvailability([FromServices] ILogger<SetupHandler> logger,
        [FromServices] ISetupInstanceManager setupInstanceManager,
        CancellationToken cancellationToken)
    {
        try
        {
            // return TypedResults.Ok(); // HTTP 200
            // return TypedResults.Created(); // HTTP 201
            // return TypedResults.NoContent(); // HTTP 204
            // return TypedResults.Conflict(); // HTTP 409
            // return TypedResults.InternalServerError(); // HTTP 500

            // 1. check the status of the setup and homebook instance
            bool setupInstanceExists = setupInstanceManager.IsSetupInstanceCreated();
            bool updateRequired = await setupInstanceManager.IsUpdateRequiredAsync(cancellationToken);
            bool setupIsFinished = setupInstanceManager.IsSetupFinishedAsync(cancellationToken);

            // 2. LATER => check dependencies like Redis, etc.
            // to do later

            if (!setupInstanceExists)
                // HTTP 200 => does not exist => setup is not executed yet and available
                return TypedResults.Ok();

            if (setupInstanceExists && !setupIsFinished && updateRequired)
                // HTTP 201 => update is required
                return TypedResults.Created();

            if (setupInstanceExists && !setupIsFinished && !updateRequired)
                // HTTP 409 => exists => setup is already running and not available
                return TypedResults.Conflict();

            if (setupInstanceExists && setupIsFinished && !updateRequired)
                // HTTP 204 => setup is finished and no update is required => Homebook is ready to use
                return TypedResults.NoContent();

            // HTPP 500 => something went wrong
            return TypedResults.InternalServerError("invalid setup configuration");
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while checking setup availability");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// returns all licenses of the project and if they are already accepted.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="licenseProvider"></param>
    /// <param name="setupConfigurationProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetLicenses([FromServices] ILogger<SetupHandler> logger,
        [FromServices] ILicenseProvider licenseProvider,
        [FromServices] ISetupConfigurationProvider setupConfigurationProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            string? licensesAreAcceptedValue = setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_ACCEPT_LICENSES);
            DependencyLicense[] licenses = await licenseProvider.GetLicensesAsync(cancellationToken);
            GetLicensesResponse response = new(
                (licensesAreAcceptedValue is not null),
                licenses
            );

            return TypedResults.Ok(response);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while checking setup availability");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// returns the database configuration if it is available (set via environment variables).
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="setupConfigurationProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetDatabaseCheck([FromServices] ILogger<SetupHandler> logger,
        [FromServices] ISetupConfigurationProvider setupConfigurationProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            string? databaseHost = setupConfigurationProvider.GetValue(EnvironmentVariables.DATABASE_HOST);
            string? databasePort = setupConfigurationProvider.GetValue(EnvironmentVariables.DATABASE_PORT);
            string? databaseName = setupConfigurationProvider.GetValue(EnvironmentVariables.DATABASE_NAME);
            string? databaseUserName = setupConfigurationProvider.GetValue(EnvironmentVariables.DATABASE_USER);
            string? databaseUserPassword = setupConfigurationProvider.GetValue(EnvironmentVariables.DATABASE_PASSWORD);


            bool databaseConfigurationFound = databaseHost is not null
                                              && databasePort is not null
                                              && databaseName is not null
                                              && databaseUserName is not null
                                              && databaseUserPassword is not null;
            if (!databaseConfigurationFound)
                return TypedResults.NotFound();

            GetDatabaseCheckResponse response = new(databaseHost,
                databasePort,
                databaseName,
                databaseUserName,
                databaseUserPassword);
            return TypedResults.Ok(response);
        }
        catch (ValidationException err)
        {
            logger.LogError(err, "Validation error while getting database configuration");
            return TypedResults.BadRequest(err.Errors.Select(x => x.ErrorMessage).ToArray());
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while checking database configuration");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// check the database connection via the given configuration.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="databaseProviderResolver"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleCheckDatabase([FromBody] CheckDatabaseRequest request,
        [FromServices] ILogger<SetupHandler> logger,
        [FromServices] IDatabaseProviderResolver databaseProviderResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            DatabaseProvider? resolvedDatabaseProvider = await databaseProviderResolver.ResolveAsync(request.DatabaseHost,
                request.DatabasePort,
                request.DatabaseName,
                request.DatabaseUserName,
                request.DatabaseUserPassword,
                cancellationToken);

            if (resolvedDatabaseProvider is not null)
                // database is available
                return TypedResults.Ok(resolvedDatabaseProvider.ToString()!.ToUpperInvariant());
            else
                // database is not available
                return TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while checking database connection");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// check if a pre-configured user is available via environment variables
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="setupConfigurationProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetPreConfiguredUser([FromServices] ILogger<SetupHandler> logger,
        [FromServices] ISetupConfigurationProvider setupConfigurationProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            string? homebookUserName = setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_USER_NAME);
            string? homebookUserPassword = setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_USER_PASSWORD);

            if (!string.IsNullOrEmpty(homebookUserName)
                && !string.IsNullOrEmpty(homebookUserPassword))
                return TypedResults.Ok();
            else
                return TypedResults.StatusCode(StatusCodes.Status404NotFound);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while migrating database");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// returns the homebook setup configuration if it is set via environment variables.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="setupConfigurationProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetConfiguration([FromServices] ILogger<SetupHandler> logger,
        [FromServices] ISetupConfigurationProvider setupConfigurationProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            string? homebookInstanceName = setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_INSTANCE_NAME);

            if (!string.IsNullOrEmpty(homebookInstanceName))
                return TypedResults.Ok();
            else
                return TypedResults.StatusCode(StatusCodes.Status404NotFound);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while migrating database");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    /// starts the database migration process.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="databaseConfigurationValidator"></param>
    /// <param name="runtimeConfigurationProvider"></param>
    /// <param name="setupInstanceManager"></param>
    /// <param name="licenseProvider"></param>
    /// <param name="setupConfigurationProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleStartSetup([FromBody] StartSetupRequest request,
        [FromServices] ILogger<SetupHandler> logger,
        [FromServices] IValidator<DatabaseConfiguration> databaseConfigurationValidator,
        [FromServices] IRuntimeConfigurationProvider runtimeConfigurationProvider,
        [FromServices] ISetupInstanceManager setupInstanceManager,
        [FromServices] ILicenseProvider licenseProvider,
        [FromServices] ISetupConfigurationProvider setupConfigurationProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. create directory structure
            setupInstanceManager.CreateRequiredDirectories();

            // 2. write license accepted file
            bool licensesAcceptedViaRequest = request.LicensesAccepted ?? false;
            bool licensesAcceptedViaEnvVar = setupConfigurationProvider.GetValue(EnvironmentVariables.HOMEBOOK_ACCEPT_LICENSES) is not null;
            if (!licensesAcceptedViaRequest
                && !licensesAcceptedViaEnvVar)
                return TypedResults.StatusCode(StatusCodes.Status422UnprocessableEntity);

            // 3. mark the licenses of this version as accepted
            await licenseProvider.MarkLicenseAsAcceptedAsync(cancellationToken);

            // 4. write database configuration to environment variables
            await UpdateDatabaseConfigurationAsync(request,
                databaseConfigurationValidator,
                runtimeConfigurationProvider,
                cancellationToken);

            // 5. save setup configuration

            // 6. write setup instance file
            await setupInstanceManager.CreateSetupInstanceAsync(cancellationToken);

            return TypedResults.Ok();
        }
        catch (ValidationException err)
        {
            logger.LogError(err, "Validation error while checking database configuration");
            return TypedResults.BadRequest(err.Errors.Select(x => x.ErrorMessage).ToArray());
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while starting the setup");
            return TypedResults.InternalServerError(err.Message);
        }
    }

    private static async Task UpdateDatabaseConfigurationAsync(StartSetupRequest request,
        IValidator<DatabaseConfiguration> databaseConfigurationValidator,
        IRuntimeConfigurationProvider runtimeConfigurationProvider,
        CancellationToken cancellationToken)
    {
        DatabaseConfiguration dbConfig = new(
            request.DatabaseType,
            request.DatabaseHost,
            request.DatabasePort,
            request.DatabaseName,
            request.DatabaseUserName,
            request.DatabaseUserPassword);
        await databaseConfigurationValidator.ValidateAndThrowAsync(dbConfig, cancellationToken);

        if (!string.IsNullOrEmpty(dbConfig.DatabaseType))
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:Provider", dbConfig.DatabaseType, cancellationToken);
        if (!string.IsNullOrEmpty(dbConfig.DatabaseHost))
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:Host", dbConfig.DatabaseHost, cancellationToken);
        if (dbConfig.DatabasePort is not null)
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:Port", dbConfig.DatabasePort, cancellationToken);
        if (!string.IsNullOrEmpty(dbConfig.DatabaseName))
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:InstanceDbName", dbConfig.DatabaseName, cancellationToken);
        if (!string.IsNullOrEmpty(dbConfig.DatabaseUserName))
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:Username", dbConfig.DatabaseUserName, cancellationToken);
        if (!string.IsNullOrEmpty(dbConfig.DatabaseUserPassword))
            // TODO: store password as encrypted value in .secret file
            await runtimeConfigurationProvider.UpdateConfigurationAsync("Database:Password", dbConfig.DatabaseUserPassword, cancellationToken);
    }
}
