using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class PomodoroView : UserControl
{
    private PomodoroViewModel? _vm;
    private Path?              _arc;
    private DispatcherTimer?   _arcTimer;

    public PomodoroView()
    {
        InitializeComponent();
        _vm         = new PomodoroViewModel();
        DataContext = _vm;

        Loaded += (_, _) =>
        {
            _arc = this.FindControl<Path>("ProgressArc");

            if (_arcTimer is null)
            {
                _arcTimer       = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
                _arcTimer.Tick += (_, _) => DrawArc();
            }

            _arcTimer.Start();
            DrawArc();
        };

        Unloaded += (_, _) => _arcTimer?.Stop();
    }

    private void DrawArc()
    {
        if (_arc is null || _vm is null) return;

        var progress = _vm.Progress;
        const double radius = 124;
        const double cx     = 124;
        const double cy     = 124;

        if (progress <= 0.001)
        {
            _arc.Data = null;
            return;
        }

        if (progress >= 0.999)
        {
            _arc.Data = Geometry.Parse(
                string.Format(CultureInfo.InvariantCulture,
                    "M {0} {1} A {2} {2} 0 1 1 {3} {1}",
                    cx, cy - radius, radius, cx - 0.01));
            return;
        }

        var angle   = progress * 360.0;
        var radians = (angle - 90.0) * Math.PI / 180.0;
        var x       = cx + radius * Math.Cos(radians);
        var y       = cy + radius * Math.Sin(radians);
        var large   = angle > 180 ? 1 : 0;

        _arc.Data = Geometry.Parse(
            string.Format(CultureInfo.InvariantCulture,
                "M {0} {1} A {2} {2} 0 {3} 1 {4} {5}",
                cx, cy - radius, radius, large, x, y));
    }
}
