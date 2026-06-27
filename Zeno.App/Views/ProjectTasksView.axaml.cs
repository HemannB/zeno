using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class ProjectTasksView : UserControl
{
    // Construtor público sem parâmetros — exigido pelo Avalonia
    public ProjectTasksView()
    {
        InitializeComponent();
    }

    // Construtor com projeto — usado na navegação
    public ProjectTasksView(ProjectItemViewModel project) : this()
    {
        DataContext = new ProjectTasksViewModel(project);
    }
}
