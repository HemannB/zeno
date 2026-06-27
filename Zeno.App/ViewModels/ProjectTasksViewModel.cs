using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class ProjectTasksViewModel : ViewModelBase
{
    private readonly int _projectId;

    [ObservableProperty] private string _projectName  = string.Empty;
    [ObservableProperty] private string _projectColor = "#6366F1";
    [ObservableProperty] private ObservableCollection<TaskItemViewModel> _tasks = [];
    [ObservableProperty] private int    _pendingCount;
    [ObservableProperty] private string _newTaskTitle = string.Empty;

    public ProjectTasksViewModel(ProjectItemViewModel project)
    {
        _projectId   = project.Id;
        ProjectName  = project.Name;
        ProjectColor = project.Color;
        Load();
    }

    private void Load()
    {
        var tasks = DataService.Instance.Tasks
            .GetByProject(_projectId)
            .Select(t => new TaskItemViewModel(t, null!));

        Tasks        = new ObservableCollection<TaskItemViewModel>(tasks);
        PendingCount = Tasks.Count(t => !t.IsCompleted);
    }

    [RelayCommand]
    private void AddTask()
    {
        var title = NewTaskTitle.Trim();
        if (string.IsNullOrEmpty(title)) return;

        var task = new ZenoTask
        {
            Title     = title,
            ProjectId = _projectId,
            DueDate   = DateTime.Today,
            Priority  = Priority.Medium
        };

        var id  = DataService.Instance.Tasks.Insert(task);
        task.Id = id;

        Tasks.Add(new TaskItemViewModel(task, null!));
        NewTaskTitle = string.Empty;
        PendingCount = Tasks.Count(t => !t.IsCompleted);
    }

    [RelayCommand]
    private void ToggleComplete(TaskItemViewModel item)
    {
        if (item.IsCompleted)
            DataService.Instance.Tasks.Uncomplete(item.Id);
        else
            DataService.Instance.Tasks.Complete(item.Id);

        item.IsCompleted = !item.IsCompleted;
        PendingCount = Tasks.Count(t => !t.IsCompleted);
    }

    [RelayCommand]
    private void Delete(TaskItemViewModel item)
    {
        DataService.Instance.Tasks.Delete(item.Id);
        Tasks.Remove(item);
        PendingCount = Tasks.Count(t => !t.IsCompleted);
    }

    [RelayCommand]
    private void GoBack() =>
        NavigationService.Instance.Navigate("Projetos");
}
