using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Zeno.App.Services;

namespace Zeno.App.ViewModels;

public partial class DayStatViewModel : ObservableObject
{
    public string Label    { get; }
    public int    Count    { get; }
    public double Progress { get; }

    public DayStatViewModel(string label, int count, int max)
    {
        Label    = label;
        Count    = count;
        Progress = max > 0 ? Math.Min(1.0, (double)count / max) : 0;
    }
}

public partial class StatsViewModel : ViewModelBase
{
    [ObservableProperty] private int    _completedToday;
    [ObservableProperty] private int    _completedWeek;
    [ObservableProperty] private int    _weeklyGoal     = 20;
    [ObservableProperty] private double _weeklyProgress;
    [ObservableProperty] private int    _streak;
    [ObservableProperty] private int    _pomodorosToday;
    [ObservableProperty] private string _weekLabel      = string.Empty;
    [ObservableProperty] private ObservableCollection<DayStatViewModel> _daySats = [];

    public StatsViewModel()
    {
        Load();
    }

    private void Load()
    {
        var today = DateTime.Today;

        // Semana atual (seg a dom)
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
            startOfWeek = today.AddDays(-6);

        WeekLabel = $"{startOfWeek:d MMM} – {startOfWeek.AddDays(6):d MMM}";

        // Tarefas completadas hoje
        CompletedToday = DataService.Instance.Tasks
            .GetCompleted(today).Count();

        // Tarefas completadas na semana
        CompletedWeek = DataService.Instance.Tasks.CountCompletedThisWeek();

        // Progresso semanal
        WeeklyProgress = WeeklyGoal > 0
            ? Math.Min(1.0, (double)CompletedWeek / WeeklyGoal)
            : 0;

        // Pomodoros hoje
        PomodorosToday = DataService.Instance.Pomodoro.CountToday();

        // Streak — dias consecutivos com pelo menos 1 tarefa concluída
        Streak = CalculateStreak();

        // Barras dos últimos 7 dias
        var days = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-6 + i))
            .ToList();

        var counts = days
            .Select(d => DataService.Instance.Tasks.GetCompleted(d).Count())
            .ToList();

        var max = counts.Max() > 0 ? counts.Max() : 1;

        DaySats = new ObservableCollection<DayStatViewModel>(
            days.Zip(counts, (d, c) => new DayStatViewModel(
                d.ToString("ddd", new CultureInfo("pt-BR")), c, max)));
    }

    private int CalculateStreak()
    {
        var streak = 0;
        var date   = DateTime.Today;

        while (true)
        {
            var count = DataService.Instance.Tasks.GetCompleted(date).Count();
            if (count == 0) break;
            streak++;
            date = date.AddDays(-1);
        }

        return streak;
    }
}
