using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class TodayViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<TaskItemViewModel> _tasks = [];
    [ObservableProperty] private ObservableCollection<TaskItemViewModel> _completedTasks = [];
    [ObservableProperty] private string _newTaskTitle  = string.Empty;
    [ObservableProperty] private string _todayLabel    = string.Empty;
    [ObservableProperty] private int    _pendingCount;
    [ObservableProperty] private int    _completedCount;
    [ObservableProperty] private bool   _hasCompleted;
    [ObservableProperty] private TaskDetailViewModel? _selectedTask;
    [ObservableProperty] private bool   _isDetailOpen;

    public TodayViewModel()
    {
        TodayLabel = DateTime.Now.ToString(
            "dddd, d 'de' MMMM", new CultureInfo("pt-BR"));
        Load();
    }

    private void Load()
    {
        var today     = DateTime.Today;
        var pending   = DataService.Instance.Tasks
            .GetByDate(today)
            .Select(t => new TaskItemViewModel(t, this));
        var completed = DataService.Instance.Tasks
            .GetCompleted(today)
            .Select(t => new TaskItemViewModel(t, this));

        Tasks          = new ObservableCollection<TaskItemViewModel>(pending);
        CompletedTasks = new ObservableCollection<TaskItemViewModel>(completed);
        UpdateCounters();
    }

    private void UpdateCounters()
    {
        PendingCount   = Tasks.Count;
        CompletedCount = CompletedTasks.Count;
        HasCompleted   = CompletedTasks.Count > 0;
    }

    [RelayCommand]
    private void AddTask()
    {
        var title = NewTaskTitle.Trim();
        if (string.IsNullOrEmpty(title)) return;

        var task = new ZenoTask
        {
            Title    = title,
            DueDate  = DateTime.Today,
            Priority = Zeno.Data.Models.Priority.Medium
        };

        var id  = DataService.Instance.Tasks.Insert(task);
        task.Id = id;

        Tasks.Add(new TaskItemViewModel(task, this));
        NewTaskTitle = string.Empty;
        UpdateCounters();
    }

    public void CompleteTask(TaskItemViewModel item)
    {
        DataService.Instance.Tasks.Complete(item.Id);
        Tasks.Remove(item);
        CompletedTasks.Insert(0, item);
        UpdateCounters();
    }

    public void UncompleteTask(TaskItemViewModel item)
    {
        CompletedTasks.Remove(item);
        Tasks.Add(item);
        UpdateCounters();
    }

    public void SelectTask(TaskItemViewModel item)
    {
        foreach (var t in Tasks.Concat(CompletedTasks))
            t.IsSelected = false;

        item.IsSelected = true;
        SelectedTask    = new TaskDetailViewModel(item, this);
        IsDetailOpen    = true;
    }

    [RelayCommand]
    private void CloseDetail()
    {
        IsDetailOpen = false;
        SelectedTask = null;
        foreach (var t in Tasks.Concat(CompletedTasks))
            t.IsSelected = false;
    }

    public void RefreshItem(TaskItemViewModel item) => item.NotifyTitleChanged();

    [RelayCommand]
    private void Refresh() => Load();
}
