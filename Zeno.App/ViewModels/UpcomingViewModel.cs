using System;
using System.Collections.Generic;
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
    [ObservableProperty] private ObservableCollection<UpcomingGroupViewModel> _groups = [];
    [ObservableProperty] private int    _totalCount;
    [ObservableProperty] private UpcomingTaskDetailViewModel? _selectedTask;
    [ObservableProperty] private bool   _isDetailOpen;

    private List<TaskItemViewModel> AllTasks =>
        Groups.SelectMany(g => g.Tasks).ToList();

    public UpcomingViewModel() => Load();

    private void Load()
    {
        var tasks = DataService.Instance.Tasks.GetUpcoming().ToList();
        TotalCount = tasks.Count;

        var grouped = tasks
            .GroupBy(t => t.DueDate?.Date ?? DateTime.Today.AddDays(1))
            .OrderBy(g => g.Key)
            .Select(g => new UpcomingGroupViewModel(FormatLabel(g.Key), g));

        Groups = new ObservableCollection<UpcomingGroupViewModel>(grouped);
    }

    private static string FormatLabel(DateTime date)
    {
        var today   = DateTime.Today;
        var diff    = (date - today).Days;
        var dayName = date.ToString("dddd, d 'de' MMMM", new CultureInfo("pt-BR"));

        return diff switch
        {
            1 => $"Amanhã — {dayName}",
            _ => diff <= 7 ? $"Esta semana — {dayName}" : dayName
        };
    }

    [RelayCommand]
    public void SelectTask(TaskItemViewModel item)
    {
        foreach (var t in AllTasks)
            t.IsSelected = false;

        item.IsSelected = true;
        SelectedTask    = new UpcomingTaskDetailViewModel(item, this);
        IsDetailOpen    = true;
    }

    [RelayCommand]
    private void CloseDetail()
    {
        IsDetailOpen = false;
        SelectedTask = null;
        foreach (var t in AllTasks)
            t.IsSelected = false;
    }

    public void RefreshItem(TaskItemViewModel item) => item.NotifyTitleChanged();

    public void DeleteItem(TaskItemViewModel item)
    {
        DataService.Instance.Tasks.Delete(item.Id);
        foreach (var g in Groups)
            g.Tasks.Remove(item);
        TotalCount = Groups.Sum(g => g.Tasks.Count);
        CloseDetailCommand.Execute(null);
    }

    [RelayCommand]
    private void Refresh() => Load();
}
