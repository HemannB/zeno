using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class TaskRepository(Database db)
{
    public IEnumerable<ZenoTask> GetByDate(DateTime date)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE date(DueDate) = @Date AND IsCompleted = 0 ORDER BY DueTime ASC",
            new { Date = date.ToString("yyyy-MM-dd") }); }
        finally { if (owned) conn.Dispose(); }
    }

    public IEnumerable<ZenoTask> GetCompleted(DateTime date)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE IsCompleted = 1 AND date(CompletedAt) = @Date ORDER BY CompletedAt DESC",
            new { Date = date.ToString("yyyy-MM-dd") }); }
        finally { if (owned) conn.Dispose(); }
    }

    public IEnumerable<ZenoTask> GetByProject(int projectId)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE ProjectId = @ProjectId ORDER BY CreatedAt DESC",
            new { ProjectId = projectId }); }
        finally { if (owned) conn.Dispose(); }
    }

    public IEnumerable<ZenoTask> GetUpcoming()
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE date(DueDate) > date('now') AND IsCompleted = 0 ORDER BY DueDate ASC"); }
        finally { if (owned) conn.Dispose(); }
    }

    public ZenoTask? GetById(int id)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.QueryFirstOrDefault<ZenoTask>(
            "SELECT * FROM Tasks WHERE Id = @Id", new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public int Insert(ZenoTask task)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            INSERT INTO Tasks (Title, Notes, Priority, Recurrence, ProjectId, DueDate, DueTime, IsFavorite)
            VALUES (@Title, @Notes, @Priority, @Recurrence, @ProjectId, @DueDate, @DueTime, @IsFavorite);
            SELECT last_insert_rowid();
            """, task); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Update(ZenoTask task)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("""
            UPDATE Tasks SET
                Title      = @Title,
                Notes      = @Notes,
                Priority   = @Priority,
                Recurrence = @Recurrence,
                ProjectId  = @ProjectId,
                DueDate    = @DueDate,
                DueTime    = @DueTime,
                IsFavorite = @IsFavorite
            WHERE Id = @Id
            """, task); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Complete(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("""
            UPDATE Tasks SET IsCompleted = 1, CompletedAt = datetime('now')
            WHERE Id = @Id
            """, new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Uncomplete(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("""
            UPDATE Tasks SET IsCompleted = 0, CompletedAt = NULL
            WHERE Id = @Id
            """, new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Delete(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("DELETE FROM Tasks WHERE Id = @Id", new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public int CountCompletedThisWeek()
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM Tasks
            WHERE IsCompleted = 1
            AND date(CompletedAt) >= date('now', 'weekday 1', '-7 days')
            """); }
        finally { if (owned) conn.Dispose(); }
    }
}
