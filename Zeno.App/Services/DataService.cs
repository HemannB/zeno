using System;
using System.IO;
using Zeno.Data;
using Zeno.Data.Repositories;

namespace Zeno.App.Services;

public class DataService
{
    private static DataService? _instance;
    public static DataService Instance => _instance ??= new DataService();

    public readonly Database Database;
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

        var dbPath = Path.Combine(appDir, "zeno.db");
        Database = new Database(dbPath);
        Tasks    = new TaskRepository(Database);
        Projects = new ProjectRepository(Database);
        Water    = new WaterRepository(Database);
        Pomodoro = new PomodoroRepository(Database);
    }
}
