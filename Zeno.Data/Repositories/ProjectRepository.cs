using Dapper;
using Zeno.Data.Models;

namespace Zeno.Data.Repositories;

public class ProjectRepository(Database db)
{
    public IEnumerable<Project> GetAll()
    {
        using var conn = db.Connect();
        return conn.Query<Project>(
            "SELECT * FROM Projects WHERE IsArchived = 0 ORDER BY Name ASC");
    }

    public Project? GetById(int id)
    {
        using var conn = db.Connect();
        return conn.QueryFirstOrDefault<Project>(
            "SELECT * FROM Projects WHERE Id = @Id", new { Id = id });
    }

    public int Insert(Project project)
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            INSERT INTO Projects (Name, Color)
            VALUES (@Name, @Color);
            SELECT last_insert_rowid();
            """, project);
    }

    public void Update(Project project)
    {
        using var conn = db.Connect();
        conn.Execute("""
            UPDATE Projects SET
                Name  = @Name,
                Color = @Color
            WHERE Id = @Id
            """, project);
    }

    public void Archive(int id)
    {
        using var conn = db.Connect();
        conn.Execute(
            "UPDATE Projects SET IsArchived = 1 WHERE Id = @Id",
            new { Id = id });
    }

    public void Delete(int id)
    {
        using var conn = db.Connect();
        conn.Execute("DELETE FROM Projects WHERE Id = @Id", new { Id = id });
    }

    public int CountTasks(int projectId)
    {
        using var conn = db.Connect();
        return conn.ExecuteScalar<int>("""
            SELECT COUNT(*) FROM Tasks
            WHERE ProjectId = @ProjectId AND IsCompleted = 0
            """, new { ProjectId = projectId });
    }
}
