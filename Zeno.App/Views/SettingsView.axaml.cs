using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }
}
