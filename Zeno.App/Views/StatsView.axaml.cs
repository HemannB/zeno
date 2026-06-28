using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class StatsView : UserControl
{
    public StatsView()
    {
        InitializeComponent();
        DataContext = new StatsViewModel();
    }
}
