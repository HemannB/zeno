using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class ProjectTasksView : UserControl
{
    public ProjectTasksView(ProjectItemViewModel project)
    {
        InitializeComponent();
        DataContext = new ProjectTasksViewModel(project);
    }
}
