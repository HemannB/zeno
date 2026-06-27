using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class ProjectRepository(Database db)
{
    public IEnumerable<Project> GetAll()
    {
        var (conn, owned) = db.Borrow();
        try { return conn.Query<Project>(
            "SELECT * FROM Projects WHERE IsArchived = 0 ORDER BY Name ASC"); }
        finally { if (owned) conn.Dispose(); }
    }

    public Project? GetById(int id)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.QueryFirstOrDefault<Project>(
            "SELECT * FROM Projects WHERE Id = @Id", new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public int Insert(Project project)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            INSERT INTO Projects (Name, Color)
            VALUES (@Name, @Color);
            SELECT last_insert_rowid();
            """, project); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Update(Project project)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("""
            UPDATE Projects SET Name = @Name, Color = @Color
            WHERE Id = @Id
            """, project); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Archive(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute(
            "UPDATE Projects SET IsArchived = 1 WHERE Id = @Id", new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public void Delete(int id)
    {
        var (conn, owned) = db.Borrow();
        try { conn.Execute("DELETE FROM Projects WHERE Id = @Id", new { Id = id }); }
        finally { if (owned) conn.Dispose(); }
    }

    public int CountTasks(int projectId)
    {
        var (conn, owned) = db.Borrow();
        try { return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM Tasks
            WHERE ProjectId = @ProjectId AND IsCompleted = 0
            """, new { ProjectId = projectId }); }
        finally { if (owned) conn.Dispose(); }
    }
}
