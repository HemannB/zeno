using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;

namespace Zeno.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _currentPage = "Hoje";

    [ObservableProperty]
    private string _greeting = GetGreeting();

    [ObservableProperty]
    private bool _isToday = true;

    [ObservableProperty]
    private bool _isProjects;

    public MainWindowViewModel()
    {
        NavigationService.Instance.Navigated += page =>
        {
            CurrentPage = page;
            IsToday     = page == "Hoje";
            IsProjects  = page == "Projetos";
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
