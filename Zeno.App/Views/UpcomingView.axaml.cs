using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class UpcomingView : UserControl
{
    public UpcomingView()
    {
        InitializeComponent();
        DataContext = new UpcomingViewModel();
    }
}
