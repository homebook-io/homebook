using System.Text;
using Microsoft.Data.Sqlite;

namespace HomeBook.UnitTests.TestCore.Helper;

public static class SqliteHelper
{
    public static string GetAllTableNames(SqliteConnection connection)
    {
        var sb = new StringBuilder();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
            ORDER BY name;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            sb.AppendLine(reader.GetString(0));
        }

        return sb.ToString();
    }

    public static List<string[]> DumpTable(SqliteConnection connection, string tableName)
    {
        var rows = new List<string[]>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {tableName};";

        using var reader = cmd.ExecuteReader();
        var fieldCount = reader.FieldCount;

        // Header hinzufügen
        var header = new string[fieldCount];
        for (int i = 0; i < fieldCount; i++)
            header[i] = reader.GetName(i);
        rows.Add(header);

        // Daten hinzufügen
        while (reader.Read())
        {
            var values = new string[fieldCount];
            for (int i = 0; i < fieldCount; i++)
                values[i] = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)!.ToString()!;
            rows.Add(values);
        }

        return rows;
    }
}
