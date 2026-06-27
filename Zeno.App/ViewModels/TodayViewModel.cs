using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly TodayViewModel _parent;
    public ZenoTask Task { get; }

    public int Id => Task.Id;
    public string Title => Task.Title;
    public Priority Priority => Task.Priority;
    public bool IsFavorite => Task.IsFavorite;
    public string? DueTime => Task.DueTime.HasValue
        ? DateTime.Today.Add(Task.DueTime.Value).ToString("HH:mm")
        : null;

    [ObservableProperty]
    private bool _isCompleted;

    public TaskItemViewModel(ZenoTask task, TodayViewModel parent)
    {
        Task = task;
        _parent = parent;
        _isCompleted = task.IsCompleted;
    }

    partial void OnIsCompletedChanged(bool value)
    {
        if (value)
            _parent.CompleteTask(this);
        else
            DataService.Instance.Tasks.Uncomplete(Task.Id);
    }
}

public partial class TodayViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<TaskItemViewModel> _tasks = [];

    [ObservableProperty]
    private ObservableCollection<TaskItemViewModel> _completedTasks = [];

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private string _todayLabel = string.Empty;

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private int _completedCount;

    public TodayViewModel()
    {
        TodayLabel = DateTime.Now.ToString("dddd, d 'de' MMMM",
            new System.Globalization.CultureInfo("pt-BR"));
        Load();
    }

    private void Load()
    {
        var today = DateTime.Today;
        var pending = DataService.Instance.Tasks.GetByDate(today)
            .Select(t => new TaskItemViewModel(t, this));
        var completed = DataService.Instance.Tasks.GetCompleted(today)
            .Select(t => new TaskItemViewModel(t, this));

        Tasks          = new ObservableCollection<TaskItemViewModel>(pending);
        CompletedTasks = new ObservableCollection<TaskItemViewModel>(completed);
        PendingCount   = Tasks.Count;
        CompletedCount = CompletedTasks.Count;
    }

    [RelayCommand]
    private void AddTask()
    {
        var title = NewTaskTitle.Trim();
        if (string.IsNullOrEmpty(title)) return;

        var task = new ZenoTask
        {
            Title   = title,
            DueDate = DateTime.Today,
            Priority = Priority.Medium
        };

        var id = DataService.Instance.Tasks.Insert(task);
        task.Id = id;

        Tasks.Add(new TaskItemViewModel(task, this));
        NewTaskTitle = string.Empty;
        PendingCount = Tasks.Count;
    }

    public void CompleteTask(TaskItemViewModel item)
    {
        DataService.Instance.Tasks.Complete(item.Id);
        Tasks.Remove(item);
        CompletedTasks.Insert(0, item);
        PendingCount   = Tasks.Count;
        CompletedCount = CompletedTasks.Count;
    }

    [RelayCommand]
    private void Refresh() => Load();
}
