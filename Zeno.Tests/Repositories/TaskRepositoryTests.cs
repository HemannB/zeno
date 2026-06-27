using Zeno.Data.Models;
using Zeno.Data.Repositories;
using Zeno.Tests.Helpers;

namespace Zeno.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly TestDatabase _testDb;
    private readonly TaskRepository _repo;
    private readonly DateTime _today = DateTime.UtcNow.Date;

    public TaskRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repo   = new TaskRepository(_testDb.Db);
    }

    [Fact]
    public void Insert_ShouldReturnNewId()
    {
        var id = _repo.Insert(new ZenoTask { Title = "Test task", DueDate = _today });
        Assert.True(id > 0);
    }

    [Fact]
    public void GetByDate_ShouldReturnTasksDueToday()
    {
        _repo.Insert(new ZenoTask { Title = "Today task",    DueDate = _today });
        _repo.Insert(new ZenoTask { Title = "Tomorrow task", DueDate = _today.AddDays(1) });

        var results = _repo.GetByDate(_today).ToList();

        Assert.Single(results);
        Assert.Equal("Today task", results[0].Title);
    }

    [Fact]
    public void Complete_ShouldMarkTaskAsCompleted()
    {
        var id = _repo.Insert(new ZenoTask { Title = "Task", DueDate = _today });
        _repo.Complete(id);

        var completed = _repo.GetCompleted(_today).ToList();
        Assert.Single(completed);
        Assert.True(completed[0].IsCompleted);
    }

    [Fact]
    public void Uncomplete_ShouldRestoreTask()
    {
        var id = _repo.Insert(new ZenoTask { Title = "Task", DueDate = _today });
        _repo.Complete(id);
        _repo.Uncomplete(id);

        var pending = _repo.GetByDate(_today).ToList();
        Assert.Single(pending);
        Assert.False(pending[0].IsCompleted);
    }

    [Fact]
    public void Delete_ShouldRemoveTask()
    {
        var id = _repo.Insert(new ZenoTask { Title = "Task", DueDate = _today });
        _repo.Delete(id);

        var task = _repo.GetById(id);
        Assert.Null(task);
    }

    [Fact]
    public void GetUpcoming_ShouldReturnFutureTasks()
    {
        _repo.Insert(new ZenoTask { Title = "Future", DueDate = _today.AddDays(3) });
        _repo.Insert(new ZenoTask { Title = "Today",  DueDate = _today });

        var upcoming = _repo.GetUpcoming().ToList();
        Assert.Single(upcoming);
        Assert.Equal("Future", upcoming[0].Title);
    }

    [Fact]
    public void Update_ShouldPersistChanges()
    {
        var id   = _repo.Insert(new ZenoTask { Title = "Old", DueDate = _today });
        var task = _repo.GetById(id)!;
        task.Title    = "New";
        task.Priority = Priority.High;
        _repo.Update(task);

        var updated = _repo.GetById(id)!;
        Assert.Equal("New", updated.Title);
        Assert.Equal(Priority.High, updated.Priority);
    }

    public void Dispose() => _testDb.Dispose();
}
