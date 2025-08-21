namespace HomeBook.Backend.Requests;

public record MigrateDatabaseRequest(string DatabaseHost,
    ushort DatabasePort,
    string DatabaseName,
    string DatabaseUserName,
    string DatabaseUserPassword);
