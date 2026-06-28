using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

// Reutiliza a mesma interface do TaskDetailViewModel
// mas com callbacks para o ProjectTasksViewModel
public partial class ProjectTaskDetailViewModel : ObservableObject
{
    private readonly TaskItemViewModel     _item;
    private readonly ProjectTasksViewModel _parent;

    [ObservableProperty] private string  _title               = string.Empty;
    [ObservableProperty] private string  _notes               = string.Empty;
    [ObservableProperty] private int     _priorityIndex;
    [ObservableProperty] private string? _dueDate;
    [ObservableProperty] private int     _selectedProjectIndex = 0;

    public List<string> Priorities   { get; } = ["Baixa", "Média", "Alta"];
    public List<string> ProjectNames { get; }
    public List<int?>   ProjectIds   { get; }

    public ProjectTaskDetailViewModel(TaskItemViewModel item, ProjectTasksViewModel parent)
    {
        _item   = item;
        _parent = parent;

        var task      = item.Task;
        Title         = task.Title;
        Notes         = task.Notes ?? string.Empty;
        PriorityIndex = (int)task.Priority;
        DueDate       = task.DueDate?.ToString("yyyy-MM-dd");

        var projects  = DataService.Instance.Projects.GetAll().ToList();
        ProjectNames  = ["Nenhum", ..projects.Select(p => p.Name)];
        ProjectIds    = [null,     ..projects.Select(p => (int?)p.Id)];

        SelectedProjectIndex = task.ProjectId.HasValue
            ? ProjectIds.IndexOf(task.ProjectId)
            : 0;
        if (SelectedProjectIndex < 0) SelectedProjectIndex = 0;
    }

    [RelayCommand]
    private void Save()
    {
        var task       = _item.Task;
        task.Title     = Title.Trim();
        task.Notes     = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
        task.Priority  = (Priority)PriorityIndex;
        task.DueDate   = DateTime.TryParse(DueDate, out var d) ? d : task.DueDate;
        task.ProjectId = SelectedProjectIndex >= 0 && SelectedProjectIndex < ProjectIds.Count
            ? ProjectIds[SelectedProjectIndex]
            : null;

        DataService.Instance.Tasks.Update(task);
        _parent.RefreshItem(_item);
        _parent.CloseDetailCommand.Execute(null);
    }

    [RelayCommand]
    private void Delete() => _parent.DeleteItem(_item);

    [RelayCommand]
    private void Cancel() => _parent.CloseDetailCommand.Execute(null);
}
