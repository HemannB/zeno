using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly TodayViewModel _parent;
    private bool _suppressChange;

    public ZenoTask Task { get; }

    public int    Id       => Task.Id;
    public string Title    => Task.Title;
    public string Priority => Task.Priority switch
    {
        Zeno.Data.Models.Priority.High   => "Alta",
        Zeno.Data.Models.Priority.Medium => "Média",
        _                                => "Baixa"
    };
    public string PriorityColor => Task.Priority switch
    {
        Zeno.Data.Models.Priority.High   => "#F43F5E",
        Zeno.Data.Models.Priority.Medium => "#F59E0B",
        _                                => "#10B981"
    };
    public string? DueTime => Task.DueTime.HasValue
        ? DateTime.Today.Add(Task.DueTime.Value).ToString("HH:mm")
        : null;

    [ObservableProperty]
    private bool _isCompleted;

    public TaskItemViewModel(ZenoTask task, TodayViewModel parent)
    {
        Task             = task;
        _parent          = parent;
        _suppressChange  = true;
        _isCompleted     = task.IsCompleted;
        _suppressChange  = false;
    }

    partial void OnIsCompletedChanged(bool value)
    {
        if (_suppressChange) return;

        if (value)
        {
            _parent.CompleteTask(this);
        }
        else
        {
            DataService.Instance.Tasks.Uncomplete(Task.Id);
            _parent.UncompleteTask(this);
        }
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

    [ObservableProperty]
    private bool _hasCompleted;

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
        PendingCount = Tasks.Count;
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

    [RelayCommand]
    private void Refresh() => Load();
}
