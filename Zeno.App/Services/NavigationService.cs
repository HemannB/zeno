using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Zeno.App.Services;

public class NavigationService
{
    private static NavigationService? _instance;
    public static NavigationService Instance => _instance ??= new NavigationService();

    public event Action<string>? Navigated;

    public void Navigate(string page) => Navigated?.Invoke(page);
}
