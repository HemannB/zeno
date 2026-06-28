using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Zeno.App.Services;

namespace Zeno.App.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _userName          = string.Empty;
    [ObservableProperty] private string _weeklyTaskGoal    = string.Empty;
    [ObservableProperty] private string _dailyWaterGoal    = string.Empty;
    [ObservableProperty] private bool   _saved;
    [ObservableProperty] private bool   _hasError;
    [ObservableProperty] private string _errorMessage      = string.Empty;

    public SettingsViewModel()
    {
        var s          = SettingsService.Instance.Current;
        UserName       = s.UserName;
        WeeklyTaskGoal = s.WeeklyTaskGoal.ToString();
        DailyWaterGoal = s.DailyWaterGoal.ToString();
    }

    [RelayCommand]
    private void Save()
    {
        HasError = false;
        Saved    = false;

        if (!int.TryParse(WeeklyTaskGoal, out var weekly) || weekly < 1)
        {
            HasError     = true;
            ErrorMessage = "Meta semanal deve ser um número maior que zero.";
            return;
        }

        if (!int.TryParse(DailyWaterGoal, out var water) || water < 1)
        {
            HasError     = true;
            ErrorMessage = "Meta de água deve ser um número maior que zero.";
            return;
        }

        SettingsService.Instance.Set(s =>
        {
            s.UserName       = UserName.Trim().Length > 0 ? UserName.Trim() : s.UserName;
            s.WeeklyTaskGoal = weekly;
            s.DailyWaterGoal = water;
        });

        Saved = true;
    }

    [RelayCommand]
    private void Reset()
    {
        var d          = new AppSettings();
        UserName       = d.UserName;
        WeeklyTaskGoal = d.WeeklyTaskGoal.ToString();
        DailyWaterGoal = d.DailyWaterGoal.ToString();
        HasError       = false;
        Save();
    }
}
