using HomeBook.Backend.Abstractions.Setup;

namespace HomeBook.Backend.Abstractions.Models;

/// <summary>
/// contains the final setup configuration for the setup process.
/// </summary>
/// <param name="DatabaseType">the type of the database which should used by the application, e.g. "PostgreSQL", "MySQL", etc.</param>
/// <param name="DatabaseHost">the hostname or IP address of the database server.</param>
/// <param name="DatabasePort">the port number on which the database server is listening.</param>
/// <param name="DatabaseName">the name of the database which should used by the application.</param>
/// <param name="DatabaseUserName">the username for accessing the database.</param>
/// <param name="DatabaseUserPassword">the password for the database user.</param>
/// <param name="HomebookConfigurationName">the username for the Homebook user, which is used as primary administrator.</param>
/// <param name="HomebookUserName">the username for the Homebook user, which is used as primary administrator.</param>
/// <param name="HomebookUserPassword">the password for the Homebook user, which is used as primary administrator.</param>
/// <param name="HomebookAcceptLicenses">if not null the licenses are accepted.</param>
public record SetupConfiguration(
    DatabaseProvider DatabaseType,
    string DatabaseHost,
    ushort DatabasePort,
    string DatabaseName,
    string DatabaseUserName,
    string DatabaseUserPassword,
    string HomebookConfigurationName,
    string HomebookUserName,
    string HomebookUserPassword,
    bool HomebookAcceptLicenses);
