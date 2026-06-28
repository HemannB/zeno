using System;
using System.Timers;

namespace Zeno.App.Services;

public enum TimerState { Idle, Focus, ShortBreak, LongBreak }

public class TimerService : IDisposable
{
    private readonly Timer _timer;
    private int _secondsRemaining;

    public TimerState State             { get; private set; } = TimerState.Idle;
    public bool        IsActive         { get; private set; } = false;
    public int         PomodorosCompleted { get; private set; } = 0;

    public int SecondsRemaining => _secondsRemaining;
    public int TotalSeconds     { get; private set; }

    public event Action? Tick;
    public event Action? SessionCompleted;

    // Lê durações do SettingsService
    private int FocusDuration      => SettingsService.Instance.Current.PomodoroFocus      * 60;
    private int ShortBreakDuration => SettingsService.Instance.Current.PomodoroShortBreak * 60;
    private int LongBreakDuration  => SettingsService.Instance.Current.PomodoroLongBreak  * 60;

    public TimerService()
    {
        _timer          = new Timer(1000);
        _timer.Elapsed += OnTick;
        SetState(TimerState.Focus);
    }

    public void Start()
    {
        if (IsActive) return;
        IsActive = true;
        _timer.Start();
    }

    public void Pause()
    {
        IsActive = false;
        _timer.Stop();
    }

    public void Reset()
    {
        Pause();
        SetState(TimerState.Focus);
        Tick?.Invoke();
    }

    public void Skip()
    {
        Pause();
        Advance();
        Tick?.Invoke();
    }

    private void OnTick(object? sender, ElapsedEventArgs e)
    {
        _secondsRemaining--;
        Tick?.Invoke();

        if (_secondsRemaining <= 0)
        {
            Pause();
            SessionCompleted?.Invoke();
            Advance();
        }
    }

    private void Advance()
    {
        if (State == TimerState.Focus)
        {
            PomodorosCompleted++;
            var isLongBreak = PomodorosCompleted % 4 == 0;
            SetState(isLongBreak ? TimerState.LongBreak : TimerState.ShortBreak);
        }
        else
        {
            SetState(TimerState.Focus);
        }
    }

    private void SetState(TimerState state)
    {
        State = state;
        TotalSeconds = state switch
        {
            TimerState.Focus      => FocusDuration,
            TimerState.ShortBreak => ShortBreakDuration,
            TimerState.LongBreak  => LongBreakDuration,
            _                     => FocusDuration
        };
        _secondsRemaining = TotalSeconds;
    }

    public double Progress =>
        TotalSeconds > 0 ? 1.0 - (double)_secondsRemaining / TotalSeconds : 0;

    public string TimeLabel =>
        $"{_secondsRemaining / 60:D2}:{_secondsRemaining % 60:D2}";

    public string StateLabel => State switch
    {
        TimerState.Focus      => "Foco",
        TimerState.ShortBreak => "Pausa curta",
        TimerState.LongBreak  => "Pausa longa",
        _                     => "Pronto"
    };

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
