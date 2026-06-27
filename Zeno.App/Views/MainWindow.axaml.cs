using Avalonia.Controls;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
