using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class UpcomingGroupViewModel : ObservableObject
{
    public string Label { get; }
    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    public UpcomingGroupViewModel(string label, IEnumerable<ZenoTask> tasks)
    {
        Label = label;
        Tasks = new ObservableCollection<TaskItemViewModel>(
            tasks.Select(t => new TaskItemViewModel(t)));
    }
}

public partial class UpcomingViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<UpcomingGroupViewModel> _groups = [];

    [ObservableProperty] private int _totalCount;

    public UpcomingViewModel() => Load();

    private void Load()
    {
        var tasks = DataService.Instance.Tasks
            .GetUpcoming()
            .ToList();

        TotalCount = tasks.Count;

        // Agrupa por data
        var grouped = tasks
            .GroupBy(t => t.DueDate?.Date ?? DateTime.Today.AddDays(1))
            .OrderBy(g => g.Key)
            .Select(g => new UpcomingGroupViewModel(FormatLabel(g.Key), g));

        Groups = new ObservableCollection<UpcomingGroupViewModel>(grouped);
    }

    private static string FormatLabel(DateTime date)
    {
        var today    = DateTime.Today;
        var diff     = (date - today).Days;
        var dayName  = date.ToString("dddd, d 'de' MMMM", new CultureInfo("pt-BR"));

        return diff switch
        {
            1 => $"Amanhã — {dayName}",
            7 => $"Em uma semana — {dayName}",
            _ => diff <= 7
                ? $"Esta semana — {dayName}"
                : dayName
        };
    }

    [RelayCommand]
    private void Refresh() => Load();
}
