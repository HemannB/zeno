using System;
using System.IO;
using Zeno.Data;
using Zeno.Data.Repositories;

namespace Zeno.App.Services;

public class DataService
{
    private static DataService? _instance;
    public static DataService Instance => _instance ??= new DataService();

    public readonly TaskRepository Tasks;
    public readonly ProjectRepository Projects;
    public readonly WaterRepository Water;
    public readonly PomodoroRepository Pomodoro;

    private DataService()
    {
        var appDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeno");

        Directory.CreateDirectory(appDir);

        var db       = new Database(Path.Combine(appDir, "zeno.db"));
        Tasks        = new TaskRepository(db);
        Projects     = new ProjectRepository(db);
        Water        = new WaterRepository(db);
        Pomodoro     = new PomodoroRepository(db);
    }
}
