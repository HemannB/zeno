using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Zeno.App.ViewModels;

namespace Zeno.App.Views;

public partial class WaterView : UserControl
{
    private WaterViewModel? _vm;
    private Path?            _arc;
    private DispatcherTimer? _arcTimer;
    private double           _displayProgress;
    private double           _targetProgress;

    public WaterView()
    {
        InitializeComponent();
        _vm         = new WaterViewModel();
        DataContext = _vm;

        Loaded += (_, _) =>
        {
            _arc = this.FindControl<Path>("WaterArc");

            if (_arcTimer is null)
            {
                _arcTimer       = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
                _arcTimer.Tick += (_, _) => Animate();
            }

            _arcTimer.Start();
        };

        Unloaded += (_, _) => _arcTimer?.Stop();
    }

    private void Animate()
    {
        if (_vm is null) return;

        _targetProgress = _vm.Progress;

        // Interpolação suave — move 12% da distância restante por frame
        var diff = _targetProgress - _displayProgress;
        if (Math.Abs(diff) > 0.001)
            _displayProgress += diff * 0.12;
        else
            _displayProgress = _targetProgress;

        DrawArc(_displayProgress);
    }

    private void DrawArc(double progress)
    {
        if (_arc is null) return;

        const double radius = 124;
        const double cx     = 124;
        const double cy     = 124;

        if (progress <= 0.001)
        {
            _arc.Data = null;
            return;
        }

        var geometry = new PathGeometry();
        var figure   = new PathFigure
        {
            StartPoint = new Point(cx, cy - radius),
            IsClosed   = false
        };

        if (progress >= 0.999)
        {
            figure.Segments!.Add(new ArcSegment
            {
                Size           = new Size(radius, radius),
                Point          = new Point(cx - 0.01, cy - radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc     = true
            });
        }
        else
        {
            var angle   = progress * 360.0;
            var radians = (angle - 90.0) * Math.PI / 180.0;
            var x       = cx + radius * Math.Cos(radians);
            var y       = cy + radius * Math.Sin(radians);

            figure.Segments!.Add(new ArcSegment
            {
                Size           = new Size(radius, radius),
                Point          = new Point(x, y),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc     = angle > 180
            });
        }

        geometry.Figures.Add(figure);
        _arc.Data = geometry;
        _arc.InvalidateMeasure();
        _arc.InvalidateVisual();
    }
}
