using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;

namespace Zeno.App.ViewModels;

public partial class PomodoroViewModel : ViewModelBase, IDisposable
{
    private readonly TimerService _timer;

    [ObservableProperty] private string _timeLabel      = "25:00";
    [ObservableProperty] private string _stateLabel     = "Pronto";
    [ObservableProperty] private string _actionLabel    = "Iniciar";
    [ObservableProperty] private double _progress       = 0;
    [ObservableProperty] private int    _pomodorosToday = 0;
    [ObservableProperty] private bool   _isActive       = false;
    [ObservableProperty] private string _stateColor     = "#6366F1";

    public PomodoroViewModel()
    {
        _timer = new TimerService();

        _timer.Tick += () => Dispatcher.UIThread.Post(UpdateUI);

        _timer.SessionCompleted += () => Dispatcher.UIThread.Post(() =>
        {
            PomodorosToday++;
            DataService.Instance.Pomodoro.Complete(
                DataService.Instance.Pomodoro.Start());
            UpdateUI();
        });

        UpdateUI();
    }

    private void UpdateUI()
    {
        TimeLabel   = _timer.TimeLabel;
        StateLabel  = _timer.StateLabel;
        Progress    = _timer.Progress;
        IsActive    = _timer.IsActive;
        ActionLabel = _timer.IsActive ? "Pausar" : "Iniciar";
        StateColor  = _timer.State switch
        {
            TimerState.Focus      => "#6366F1",
            TimerState.ShortBreak => "#10B981",
            TimerState.LongBreak  => "#0EA5E9",
            _                     => "#6366F1"
        };
    }

    [RelayCommand]
    private void ToggleTimer()
    {
        if (_timer.IsActive) _timer.Pause();
        else                 _timer.Start();
        UpdateUI();
    }

    [RelayCommand]
    private void Reset()
    {
        _timer.Reset();
        UpdateUI();
    }

    [RelayCommand]
    private void Skip()
    {
        _timer.Skip();
        UpdateUI();
    }

    public void Dispose() => _timer.Dispose();
}
