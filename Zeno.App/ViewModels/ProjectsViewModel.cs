using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class ProjectItemViewModel : ObservableObject
{
    public Project Project { get; }
    public int    Id    => Project.Id;
    public string Name  => Project.Name;
    public string Color => Project.Color;
    public int    Count { get; private set; }

    public ProjectItemViewModel(Project project)
    {
        Project = project;
        Count   = DataService.Instance.Projects.CountTasks(project.Id);
    }
}

public partial class ProjectsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProjectItemViewModel> _projects = [];

    [ObservableProperty]
    private string _newProjectName = string.Empty;

    [ObservableProperty]
    private string _selectedColor = "#6366F1";

    [ObservableProperty]
    private bool _isAdding;

    public List<string> AvailableColors { get; } =
    [
        "#6366F1", "#8B5CF6", "#0EA5E9", "#10B981",
        "#F59E0B", "#F43F5E", "#EC4899", "#F97316",
        "#14B8A6", "#64748B"
    ];

    public ProjectsViewModel() => Load();

    private void Load()
    {
        var items = DataService.Instance.Projects
            .GetAll()
            .Select(p => new ProjectItemViewModel(p));
        Projects = new ObservableCollection<ProjectItemViewModel>(items);
    }

    [RelayCommand]
    private void StartAdding()
    {
        NewProjectName = string.Empty;
        SelectedColor  = "#6366F1";
        IsAdding       = true;
    }

    [RelayCommand]
    private void CancelAdding() => IsAdding = false;

    [RelayCommand]
    private void AddProject()
    {
        var name = NewProjectName.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var project = new Project { Name = name, Color = SelectedColor };
        var id      = DataService.Instance.Projects.Insert(project);
        project.Id  = id;

        Projects.Add(new ProjectItemViewModel(project));
        IsAdding = false;
    }

    [RelayCommand]
    private void Archive(ProjectItemViewModel item)
    {
        DataService.Instance.Projects.Archive(item.Id);
        Projects.Remove(item);
    }
}
