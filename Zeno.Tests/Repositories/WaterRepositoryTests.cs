using Zeno.Data.Repositories;
using Zeno.Tests.Helpers;

namespace Zeno.Tests.Repositories;

public class WaterRepositoryTests : IDisposable
{
    private readonly TestDatabase _testDb;
    private readonly WaterRepository _repo;

    public WaterRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repo   = new WaterRepository(_testDb.Db);
    }

    [Fact]
    public void GetToday_ShouldCreateLogIfNotExists()
    {
        var log = _repo.GetToday();
        Assert.NotNull(log);
        Assert.Equal(0, log.Glasses);
        Assert.Equal(8, log.Goal);
    }

    [Fact]
    public void AddGlass_ShouldIncrementCount()
    {
        var log = _repo.GetToday();
        _repo.AddGlass(log.Id);
        _repo.AddGlass(log.Id);

        var updated = _repo.GetToday();
        Assert.Equal(2, updated.Glasses);
    }

    [Fact]
    public void RemoveGlass_ShouldDecrementCount()
    {
        var log = _repo.GetToday();
        _repo.AddGlass(log.Id);
        _repo.AddGlass(log.Id);
        _repo.RemoveGlass(log.Id);

        var updated = _repo.GetToday();
        Assert.Equal(1, updated.Glasses);
    }

    [Fact]
    public void RemoveGlass_ShouldNotGoBelowZero()
    {
        var log = _repo.GetToday();
        _repo.RemoveGlass(log.Id);

        var updated = _repo.GetToday();
        Assert.Equal(0, updated.Glasses);
    }

    [Fact]
    public void SetGoal_ShouldPersistNewGoal()
    {
        var log = _repo.GetToday();
        _repo.SetGoal(log.Id, 10);

        var updated = _repo.GetToday();
        Assert.Equal(10, updated.Goal);
    }

    public void Dispose() => _testDb.Dispose();
}
