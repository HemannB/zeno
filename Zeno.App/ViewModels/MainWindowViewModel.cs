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
    [ObservableProperty] private string  _currentPage  = "Hoje";
    [ObservableProperty] private string  _greeting     = GetGreeting();
    [ObservableProperty] private Control _currentView;

    [ObservableProperty] private string _todayBg     = "#1E1E3A";
    [ObservableProperty] private string _upcomingBg  = "Transparent";
    [ObservableProperty] private string _projectsBg  = "Transparent";
    [ObservableProperty] private string _pomodorooBg = "Transparent";
    [ObservableProperty] private string _waterBg     = "Transparent";
    [ObservableProperty] private string _statsBg     = "Transparent";
    [ObservableProperty] private string _todayFg     = "#818CF8";
    [ObservableProperty] private string _upcomingFg  = "#9494A3";
    [ObservableProperty] private string _projectsFg  = "#9494A3";
    [ObservableProperty] private string _pomodoroFg  = "#9494A3";
    [ObservableProperty] private string _waterFg     = "#9494A3";
    [ObservableProperty] private string _statsFg     = "#9494A3";

    private readonly Dictionary<string, Control> _views = new();

    public MainWindowViewModel()
    {
        _currentView = GetOrCreate("Hoje");
        NavigationService.Instance.Navigated += SetPage;
    }

    private void SetPage(string page)
    {
        CurrentPage = page;

        TodayBg     = page == "Hoje"          ? "#1E1E3A" : "Transparent";
        UpcomingBg  = page == "Próximos"      ? "#1E1E3A" : "Transparent";
        ProjectsBg  = page == "Projetos"      ? "#1E1E3A" : "Transparent";
        PomodorooBg = page == "Pomodoro"      ? "#1E1E3A" : "Transparent";
        WaterBg     = page == "Hidratação"    ? "#1E1E3A" : "Transparent";
        StatsBg     = page == "Estatísticas"  ? "#1E1E3A" : "Transparent";
        TodayFg     = page == "Hoje"          ? "#818CF8" : "#9494A3";
        UpcomingFg  = page == "Próximos"      ? "#818CF8" : "#9494A3";
        ProjectsFg  = page == "Projetos"      ? "#818CF8" : "#9494A3";
        PomodoroFg  = page == "Pomodoro"      ? "#818CF8" : "#9494A3";
        WaterFg     = page == "Hidratação"    ? "#818CF8" : "#9494A3";
        StatsFg     = page == "Estatísticas"  ? "#818CF8" : "#9494A3";

        if (page == "Hoje")         _views.Remove("Hoje");
        if (page == "Próximos")     _views.Remove("Próximos");
        if (page == "Projetos")     _views.Remove("Projetos");
        if (page == "Hidratação")   _views.Remove("Hidratação");
        if (page == "Estatísticas") _views.Remove("Estatísticas");

        CurrentView = GetOrCreate(page);
    }

    private Control GetOrCreate(string page)
    {
        if (_views.TryGetValue(page, out var cached))
            return cached;

        Control view = page switch
        {
            "Próximos"     => new UpcomingView(),
            "Projetos"     => CreateProjectsView(),
            "Pomodoro"     => new PomodoroView(),
            "Hidratação"   => new WaterView(),
            "Estatísticas" => new StatsView(),
            _              => new TodayView()
        };

        _views[page] = view;
        return view;
    }

    private ProjectsView CreateProjectsView()
    {
        var view = new ProjectsView();

        if (view.DataContext is ProjectsViewModel vm)
        {
            vm.OnOpenProject = project =>
            {
                var projectView = new ProjectTasksView(project);
                CurrentPage = project.Name;
                CurrentView = projectView;

                TodayBg    = "Transparent";
                ProjectsBg = "Transparent";
                TodayFg    = "#9494A3";
                ProjectsFg = "#9494A3";
            };
        }

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
