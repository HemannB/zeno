using System.IO;
using System.Reflection;
using Dapper;
using Microsoft.Data.Sqlite;
using Zeno.Data;

namespace Zeno.Tests.Helpers;

public class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    public Database Db { get; }

    public TestDatabase()
    {
        // Mantém a conexão aberta — banco in-memory só existe enquanto ela estiver ativa
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Roda a migration direto na conexão aberta
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sqlPath = Path.Combine(baseDir, "Migrations", "001_init.sql");
        var sql     = File.ReadAllText(sqlPath);
        _connection.Execute(sql);

        Db = new Database(_connection);
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
