using Dapper;
using Microsoft.Data.Sqlite;

namespace Zeno.Data;

public class Database
{
    private readonly string _connectionString;

    public Database(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        Initialize();
    }

    public SqliteConnection Connect() => new(_connectionString);

    private void Initialize()
    {
        var baseDir = Path.GetDirectoryName(typeof(Database).Assembly.Location)!;
        var sqlPath = Path.Combine(baseDir, "Migrations", "001_init.sql");
        var sql = File.ReadAllText(sqlPath);

        using var conn = Connect();
        conn.Open();
        conn.Execute(sql);
    }
}
