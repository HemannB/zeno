using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class WaterRepository(Database db)
{
    public WaterLog GetToday()
    {
        var (conn, owned) = db.Borrow();
        try
        {
            var log = conn.QueryFirstOrDefault<WaterLog>(
                "SELECT * FROM WaterLog WHERE date(Date) = date('now')");
            if (log is not null) return log;

            var id = conn.ExecuteScalar<int>("""
                INSERT INTO WaterLog (Glasses, Goal, Date)
                VALUES (0, 8, date('now'));
                SELECT last_insert_rowid();
                """);
            return new WaterLog { Id = id };
        }
        finally { if (owned) conn.Dispose(); }
    }

    public void AddGlass(int logId)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute(
            "UPDATE WaterLog SET Glasses = Glasses + 1 WHERE Id = @Id", new { Id = logId }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void RemoveGlass(int logId)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute(
            "UPDATE WaterLog SET Glasses = MAX(0, Glasses - 1) WHERE Id = @Id", new { Id = logId }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void SetGoal(int logId, int goal)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute(
            "UPDATE WaterLog SET Goal = @Goal WHERE Id = @Id", new { Id = logId, Goal = goal }); }
        finally { if (owned) conn.Dispose(); }
    }
}
