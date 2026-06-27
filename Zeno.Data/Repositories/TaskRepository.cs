using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class TaskRepository(Database db)
{
    public IEnumerable<ZenoTask> GetByDate(DateTime date)
    {
        using var conn = db.Connect();
        return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE date(DueDate) = date(@Date) AND IsCompleted = 0 ORDER BY DueTime ASC",
            new { Date = date.ToString("yyyy-MM-dd") });
    }

    public IEnumerable<ZenoTask> GetCompleted(DateTime date)
    {
        using var conn = db.Connect();
        return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE date(CompletedAt) = date(@Date) ORDER BY CompletedAt DESC",
            new { Date = date.ToString("yyyy-MM-dd") });
    }

    public IEnumerable<ZenoTask> GetByProject(int projectId)
    {
        using var conn = db.Connect();
        return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE ProjectId = @ProjectId ORDER BY CreatedAt DESC",
            new { ProjectId = projectId });
    }

    public IEnumerable<ZenoTask> GetUpcoming()
    {
        using var conn = db.Connect();
        return conn.Query<ZenoTask>(
            "SELECT * FROM Tasks WHERE date(DueDate) > date('now') AND IsCompleted = 0 ORDER BY DueDate ASC");
    }

    public ZenoTask? GetById(int id)
    {
        using var conn = db.Connect();
        return conn.QueryFirstOrDefault<ZenoTask>(
            "SELECT * FROM Tasks WHERE Id = @Id", new { Id = id });
    }

    public int Insert(ZenoTask task)
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            INSERT INTO Tasks (Title, Notes, Priority, Recurrence, ProjectId, DueDate, DueTime, IsFavorite)
            VALUES (@Title, @Notes, @Priority, @Recurrence, @ProjectId, @DueDate, @DueTime, @IsFavorite);
            SELECT last_insert_rowid();
            """, task);
    }

    public void Update(ZenoTask task)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE Tasks SET
                Title       = @Title,
                Notes       = @Notes,
                Priority    = @Priority,
                Recurrence  = @Recurrence,
                ProjectId   = @ProjectId,
                DueDate     = @DueDate,
                DueTime     = @DueTime,
                IsFavorite  = @IsFavorite
            WHERE Id = @Id
            """, task);
    }

    public void Complete(int id)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE Tasks SET
                IsCompleted = 1,
                CompletedAt = datetime('now')
            WHERE Id = @Id
            """, new { Id = id });
    }

    public void Uncomplete(int id)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE Tasks SET
                IsCompleted = 0,
                CompletedAt = NULL
            WHERE Id = @Id
            """, new { Id = id });
    }

    public void Delete(int id)
    {
        using var conn = db.Connect();
        conn.Execute("DELETE FROM Tasks WHERE Id = @Id", new { Id = id });
    }

    public int CountCompletedThisWeek()
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM Tasks
            WHERE IsCompleted = 1
            AND date(CompletedAt) >= date('now', 'weekday 1', '-7 days')
            """);
    }
}
