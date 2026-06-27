using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly TodayViewModel? _parent;
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

    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private bool _isSelected;

    // Construtor com parent — usado no TodayView
    public TaskItemViewModel(ZenoTask task, TodayViewModel parent)
    {
        Task            = task;
        _parent         = parent;
        _suppressChange = true;
        _isCompleted    = task.IsCompleted;
        _suppressChange = false;
    }

    // Construtor standalone — usado em ProjectTasksView
    public TaskItemViewModel(ZenoTask task)
    {
        Task            = task;
        _parent         = null;
        _suppressChange = true;
        _isCompleted    = task.IsCompleted;
        _suppressChange = false;
    }

    partial void OnIsCompletedChanged(bool value)
    {
        if (_suppressChange) return;
        if (_parent is not null)
        {
            if (value) _parent.CompleteTask(this);
            else
            {
                DataService.Instance.Tasks.Uncomplete(Task.Id);
                _parent.UncompleteTask(this);
            }
        }
    }

    public void NotifyTitleChanged()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(PriorityColor));
    }

    [RelayCommand]
    private void Select() => _parent?.SelectTask(this);
}
