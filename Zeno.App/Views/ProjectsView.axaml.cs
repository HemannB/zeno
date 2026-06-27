using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class ProjectsView : UserControl
{
    public ProjectsView()
    {
        InitializeComponent();
        DataContext = new ProjectsViewModel();
    }
}
