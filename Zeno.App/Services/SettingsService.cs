using System;
using System.IO;
using System.Text.Json;

namespace Zeno.App.Services;

public class AppSettings
{
    public int    WeeklyTaskGoal  { get; set; } = 20;
    public int    DailyWaterGoal  { get; set; } = 8;
    public string UserName        { get; set; } = "Bruno";
    public bool   DarkMode        { get; set; } = true;
}

public class SettingsService
{
    private static SettingsService? _instance;
    public static SettingsService Instance => _instance ??= new SettingsService();

    private readonly string _path;
    public AppSettings Current { get; private set; }

    private SettingsService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeno");

        Directory.CreateDirectory(dir);
        _path   = Path.Combine(dir, "settings.json");
        Current = Load();
    }

    private AppSettings Load()
    {
        try
        {
            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { /* usa padrão */ }

        return new AppSettings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_path, json);
    }

    public void Set(Action<AppSettings> update)
    {
        update(Current);
        Save();
    }
}
