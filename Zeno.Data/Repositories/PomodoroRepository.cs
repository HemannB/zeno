using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class PomodoroRepository(Database db)
{
    public int Start(int? taskId = null)
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            INSERT INTO PomodoroSessions (TaskId, StartedAt)
            VALUES (@TaskId, datetime('now'));
            SELECT last_insert_rowid();
            """, new { TaskId = taskId });
    }

    public void Complete(int id)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE PomodoroSessions SET
                CompletedAt  = datetime('now'),
                WasCompleted = 1
            WHERE Id = @Id
            """, new { Id = id });
    }

    public int CountToday()
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM PomodoroSessions
            WHERE date(StartedAt) = date('now') AND WasCompleted = 1
            """);
    }

    public IEnumerable<PomodoroSession> GetByTask(int taskId)
    {
        using var conn = db.Connect();
        return conn.Query<PomodoroSession>(
            "SELECT * FROM PomodoroSessions WHERE TaskId = @TaskId ORDER BY StartedAt DESC",
            new { TaskId = taskId });
    }
}
