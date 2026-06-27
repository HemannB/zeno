using Zeno.Data.Repositories;
using Zeno.Tests.Helpers;

namespace Zeno.Tests.Repositories;

public class PomodoroRepositoryTests : IDisposable
{
    private readonly TestDatabase _testDb;
    private readonly PomodoroRepository _repo;

    public PomodoroRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repo   = new PomodoroRepository(_testDb.Db);
    }

    [Fact]
    public void Start_ShouldReturnNewId()
    {
        var id = _repo.Start();
        Assert.True(id > 0);
    }

    [Fact]
    public void Start_WithTaskId_ShouldAssociateTask()
    {
        var taskRepo = new TaskRepository(_testDb.Db);
        var taskId   = taskRepo.Insert(new Zeno.Data.Models.ZenoTask
        {
            Title   = "Task",
            DueDate = DateTime.Today
        });

        var sessionId = _repo.Start(taskId);
        Assert.True(sessionId > 0);
    }

    [Fact]
    public void Complete_ShouldMarkSessionAsCompleted()
    {
        var id = _repo.Start();
        _repo.Complete(id);

        var count = _repo.CountToday();
        Assert.Equal(1, count);
    }

    [Fact]
    public void CountToday_ShouldNotCountIncomplete()
    {
        _repo.Start(); // não completa
        var count = _repo.CountToday();
        Assert.Equal(0, count);
    }

    [Fact]
    public void GetByTask_ShouldReturnSessionsForTask()
    {
        var taskRepo = new TaskRepository(_testDb.Db);
        var taskId   = taskRepo.Insert(new Zeno.Data.Models.ZenoTask
        {
            Title   = "Task",
            DueDate = DateTime.Today
        });

        var s1 = _repo.Start(taskId);
        var s2 = _repo.Start(taskId);
        _repo.Complete(s1);
        _repo.Complete(s2);

        var sessions = _repo.GetByTask(taskId).ToList();
        Assert.Equal(2, sessions.Count);
    }

    public void Dispose() => _testDb.Dispose();
}
