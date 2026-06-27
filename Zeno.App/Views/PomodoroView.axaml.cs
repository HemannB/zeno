using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class PomodoroView : UserControl
{
    private PomodoroViewModel? _vm;
    private Path? _arc;
    private DispatcherTimer? _arcTimer;

    public PomodoroView()
    {
        InitializeComponent();
        _vm = new PomodoroViewModel();
        DataContext = _vm;

        Loaded += (_, _) =>
        {
            _arc = this.FindControl<Path>("ProgressArc");
            _arcTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _arcTimer.Tick += (_, _) => DrawArc();
            _arcTimer.Start();
        };

        Unloaded += (_, _) =>
        {
            _arcTimer?.Stop();
            _vm?.Dispose();
        };
    }

    private void DrawArc()
    {
        if (_arc is null || _vm is null) return;

        var progress = _vm.Progress;
        const double radius = 124;
        const double cx = 124;
        const double cy = 124;

        var angle   = progress * 360.0;
        var radians = (angle - 90) * Math.PI / 180.0;
        var x       = cx + radius * Math.Cos(radians);
        var y       = cy + radius * Math.Sin(radians);
        var large   = angle > 180 ? 1 : 0;

        if (progress >= 0.999)
        {
            _arc.Data = Geometry.Parse(
                $"M {cx} {cy - radius} " +
                $"A {radius} {radius} 0 1 1 {cx - 0.01} {cy - radius}");
        }
        else if (progress <= 0.001)
        {
            _arc.Data = null;
        }
        else
        {
            _arc.Data = Geometry.Parse(
                $"M {cx} {cy - radius} " +
                $"A {radius} {radius} 0 {large} 1 {x:F2} {y:F2}");
        }
    }
}
