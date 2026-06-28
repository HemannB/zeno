using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;
using Zeno.Data.Models;

namespace Zeno.App.ViewModels;

public partial class WaterViewModel : ViewModelBase
{
    private WaterLog _log;
    private readonly DispatcherTimer _resetTimer;

    [ObservableProperty] private int    _glasses;
    [ObservableProperty] private int    _goal;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _statusLabel   = string.Empty;
    [ObservableProperty] private bool   _isGoalReached;
    [ObservableProperty] private string _quote         = string.Empty;

    public WaterViewModel()
    {
        _log  = DataService.Instance.Water.GetToday();
        Quote = QuoteService.GetWaterQuote();

        // Sincroniza meta do SettingsService com o banco
        var settingsGoal = SettingsService.Instance.Current.DailyWaterGoal;
        if (_log.Goal != settingsGoal)
        {
            DataService.Instance.Water.SetGoal(_log.Id, settingsGoal);
            _log.Goal = settingsGoal;
        }

        Refresh();

        _resetTimer       = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
        _resetTimer.Tick += (_, _) => CheckDayReset();
        _resetTimer.Start();
    }

    private void Refresh()
    {
        Glasses       = _log.Glasses;
        Goal          = _log.Goal;
        Progress      = Goal > 0 ? Math.Min(1.0, (double)Glasses / Goal) : 0;
        IsGoalReached = Glasses >= Goal;
        StatusLabel   = IsGoalReached
            ? "Meta atingida! 🎉"
            : $"{Glasses} de {Goal} copos";
    }

    private void CheckDayReset()
    {
        var today = DataService.Instance.Water.GetToday();
        if (today.Id != _log.Id)
        {
            _log  = today;
            Quote = QuoteService.GetWaterQuote();

            // Aplica meta do settings no novo dia
            var settingsGoal = SettingsService.Instance.Current.DailyWaterGoal;
            if (_log.Goal != settingsGoal)
            {
                DataService.Instance.Water.SetGoal(_log.Id, settingsGoal);
                _log.Goal = settingsGoal;
            }

            Refresh();
        }
    }

    [RelayCommand]
    private void AddGlass()
    {
        if (_log.Glasses >= _log.Goal * 2) return;
        DataService.Instance.Water.AddGlass(_log.Id);
        _log.Glasses++;
        Refresh();
    }

    [RelayCommand]
    private void RemoveGlass()
    {
        if (_log.Glasses <= 0) return;
        DataService.Instance.Water.RemoveGlass(_log.Id);
        _log.Glasses--;
        Refresh();
    }
}
