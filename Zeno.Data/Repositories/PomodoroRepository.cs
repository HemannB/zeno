using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class PomodoroRepository(Database db)
{
    public int Start(int? taskId = null)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            INSERT INTO PomodoroSessions (TaskId, StartedAt)
            VALUES (@TaskId, datetime('now'));
            SELECT last_insert_rowid();
            """, new { TaskId = taskId }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Complete(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("""
            UPDATE PomodoroSessions SET CompletedAt = datetime('now'), WasCompleted = 1
            WHERE Id = @Id
            """, new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public int CountToday()
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM PomodoroSessions
            WHERE date(StartedAt) = date('now') AND WasCompleted = 1
            """); }
        finally { if (owned) conn.Dispose(); }
    }

    public IEnumerable<PomodoroSession> GetByTask(int taskId)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<PomodoroSession>(
            "SELECT * FROM PomodoroSessions WHERE TaskId = @TaskId ORDER BY StartedAt DESC",
            new { TaskId = taskId }); }
        finally { if (owned) conn.Dispose(); }
    }
}
