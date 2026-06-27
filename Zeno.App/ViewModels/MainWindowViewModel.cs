using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.App.Views;

namespace Zeno.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string  _currentPage = "Hoje";
    [ObservableProperty] private string  _greeting    = GetGreeting();
    [ObservableProperty] private Control _currentView = new TodayView();

    [ObservableProperty] private string _todayBg    = "#1E1E3A";
    [ObservableProperty] private string _projectsBg = "Transparent";
    [ObservableProperty] private string _todayFg    = "#818CF8";
    [ObservableProperty] private string _projectsFg = "#9494A3";

    public MainWindowViewModel()
    {
        NavigationService.Instance.Navigated += SetPage;
    }

    private void SetPage(string page)
    {
        CurrentPage = page;

        TodayBg    = page == "Hoje"     ? "#1E1E3A"  : "Transparent";
        ProjectsBg = page == "Projetos" ? "#1E1E3A"  : "Transparent";
        TodayFg    = page == "Hoje"     ? "#818CF8"  : "#9494A3";
        ProjectsFg = page == "Projetos" ? "#818CF8"  : "#9494A3";

        CurrentView = page switch
        {
            "Projetos" => new ProjectsView(),
            _          => new TodayView()
        };
    }

    [RelayCommand]
    private void Navigate(string page) => NavigationService.Instance.Navigate(page);

    private static string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour < 12 ? "Bom dia" : hour < 18 ? "Boa tarde" : "Boa noite";
    }
}
