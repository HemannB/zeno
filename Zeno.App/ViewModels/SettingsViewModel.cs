using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;

namespace Zeno.App.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _userName       = string.Empty;
    [ObservableProperty] private int    _weeklyTaskGoal;
    [ObservableProperty] private int    _dailyWaterGoal;
    [ObservableProperty] private bool   _saved;

    public SettingsViewModel()
    {
        var s       = SettingsService.Instance.Current;
        UserName    = s.UserName;
        WeeklyTaskGoal = s.WeeklyTaskGoal;
        DailyWaterGoal = s.DailyWaterGoal;
    }

    [RelayCommand]
    private void Save()
    {
        SettingsService.Instance.Set(s =>
        {
            s.UserName       = UserName.Trim().Length > 0 ? UserName.Trim() : s.UserName;
            s.WeeklyTaskGoal = WeeklyTaskGoal > 0 ? WeeklyTaskGoal : s.WeeklyTaskGoal;
            s.DailyWaterGoal = DailyWaterGoal > 0 ? DailyWaterGoal : s.DailyWaterGoal;
        });

        Saved = true;
    }

    [RelayCommand]
    private void Reset()
    {
        var defaults   = new AppSettings();
        UserName       = defaults.UserName;
        WeeklyTaskGoal = defaults.WeeklyTaskGoal;
        DailyWaterGoal = defaults.DailyWaterGoal;
        Save();
    }
}
