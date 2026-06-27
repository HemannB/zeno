using Zeno.Data.Models;
using Zeno.Data.Repositories;
using Zeno.Tests.Helpers;

namespace Zeno.Tests.Repositories;

public class ProjectRepositoryTests : IDisposable
{
    private readonly TestDatabase _testDb;
    private readonly ProjectRepository _repo;

    public ProjectRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repo   = new ProjectRepository(_testDb.Db);
    }

    [Fact]
    public void Insert_ShouldReturnNewId()
    {
        var id = _repo.Insert(new Project { Name = "Work", Color = "#6366F1" });
        Assert.True(id > 0);
    }

    [Fact]
    public void GetAll_ShouldReturnOnlyActiveProjects()
    {
        var id = _repo.Insert(new Project { Name = "Active" });
        _repo.Insert(new Project { Name = "Archived" });
        _repo.Archive(_repo.GetAll().Last().Id);

        var all = _repo.GetAll().ToList();
        Assert.Single(all);
        Assert.Equal("Active", all[0].Name);
    }

    [Fact]
    public void Update_ShouldPersistChanges()
    {
        var id      = _repo.Insert(new Project { Name = "Old", Color = "#000000" });
        var project = _repo.GetById(id)!;
        project.Name  = "New";
        project.Color = "#FFFFFF";
        _repo.Update(project);

        var updated = _repo.GetById(id)!;
        Assert.Equal("New", updated.Name);
        Assert.Equal("#FFFFFF", updated.Color);
    }

    [Fact]
    public void CountTasks_ShouldReturnPendingTaskCount()
    {
        var projectId  = _repo.Insert(new Project { Name = "Work" });
        var taskRepo   = new TaskRepository(_testDb.Db);

        taskRepo.Insert(new ZenoTask { Title = "T1", ProjectId = projectId, DueDate = DateTime.Today });
        taskRepo.Insert(new ZenoTask { Title = "T2", ProjectId = projectId, DueDate = DateTime.Today });
        var t3 = taskRepo.Insert(new ZenoTask { Title = "T3", ProjectId = projectId, DueDate = DateTime.Today });
        taskRepo.Complete(t3);

        var count = _repo.CountTasks(projectId);
        Assert.Equal(2, count);
    }

    public void Dispose() => _testDb.Dispose();
}
