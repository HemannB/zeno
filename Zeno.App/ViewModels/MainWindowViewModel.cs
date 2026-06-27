using System;
using System.Collections.Generic;
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
    [ObservableProperty] private Control _currentView;

    [ObservableProperty] private string _todayBg    = "#1E1E3A";
    [ObservableProperty] private string _projectsBg = "Transparent";
    [ObservableProperty] private string _pomodorooBg = "Transparent";
    [ObservableProperty] private string _todayFg    = "#818CF8";
    [ObservableProperty] private string _projectsFg = "#9494A3";
    [ObservableProperty] private string _pomodoroFg = "#9494A3";

    // Cache de views — evita recriar e perder estado
    private readonly Dictionary<string, Control> _views = new();

    public MainWindowViewModel()
    {
        _currentView = GetOrCreate("Hoje");
        NavigationService.Instance.Navigated += SetPage;
    }

    private void SetPage(string page)
    {
        CurrentPage = page;

        TodayBg     = page == "Hoje"     ? "#1E1E3A" : "Transparent";
        ProjectsBg  = page == "Projetos" ? "#1E1E3A" : "Transparent";
        PomodorooBg = page == "Pomodoro" ? "#1E1E3A" : "Transparent";
        TodayFg     = page == "Hoje"     ? "#818CF8" : "#9494A3";
        ProjectsFg  = page == "Projetos" ? "#818CF8" : "#9494A3";
        PomodoroFg  = page == "Pomodoro" ? "#818CF8" : "#9494A3";

        CurrentView = GetOrCreate(page);
    }

    private Control GetOrCreate(string page)
    {
        if (_views.TryGetValue(page, out var cached))
            return cached;

        Control view = page switch
        {
            "Projetos" => new ProjectsView(),
            "Pomodoro" => new PomodoroView(),
            _          => new TodayView()
        };

        _views[page] = view;
        return view;
    }

    [RelayCommand]
    private void Navigate(string page) => NavigationService.Instance.Navigate(page);

    private static string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour < 12 ? "Bom dia" : hour < 18 ? "Boa tarde" : "Boa noite";
    }
}
