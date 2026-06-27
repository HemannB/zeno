using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class WaterRepository(Database db)
{
    public WaterLog GetToday()
    {
        using var conn = db.Connect();
        var log = conn.QueryFirstOrDefault<WaterLog>(
            "SELECT * FROM WaterLog WHERE date(Date) = date('now')");

        if (log is not null) return log;

        // Cria o registro do dia se não existir
        var id = conn.ExecuteScalar<int>("""
            INSERT INTO WaterLog (Glasses, Goal, Date)
            VALUES (0, 8, date('now'));
            SELECT last_insert_rowid();
            """);

        return new WaterLog { Id = id };
    }

    public void AddGlass(int logId)
    {
        using var conn = db.Connect();
        conn.Execute(
            "UPDATE WaterLog SET Glasses = Glasses + 1 WHERE Id = @Id",
            new { Id = logId });
    }

    public void RemoveGlass(int logId)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE WaterLog SET Glasses = MAX(0, Glasses - 1) WHERE Id = @Id
            """, new { Id = logId });
    }

    public void SetGoal(int logId, int goal)
    {
        using var conn = db.Connect();
        conn.Execute(
            "UPDATE WaterLog SET Goal = @Goal WHERE Id = @Id",
            new { Id = logId, Goal = goal });
    }
}
