using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Zeno.Data;

public class Database
{
    private readonly string? _connectionString;
    private readonly SqliteConnection? _fixedConnection;

    public Database(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        Initialize();
    }

    public Database(SqliteConnection connection)
    {
        _fixedConnection = connection;
    }

    /// <summary>
    /// Retorna uma conexão pronta para uso.
    /// Em modo teste retorna a conexão fixa (não feche-a).
    /// Em modo produção retorna uma nova conexão (use com using).
    /// </summary>
    public (SqliteConnection conn, bool owned) Borrow()
    {
        if (_fixedConnection is not null)
            return (_fixedConnection, false);

        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return (conn, true);
    }

    private void Initialize()
    {
        var (conn, owned) = Borrow();
        try
        {
            var baseDir = Path.GetDirectoryName(typeof(Database).Assembly.Location)!;
            var sql     = File.ReadAllText(Path.Combine(baseDir, "Migrations", "001_init.sql"));
            conn.Execute(sql);
        }
        finally
        {
            if (owned) conn.Dispose();
        }
    }
}
