using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Dota2Helper.Views;

public partial class MainView : UserControl
{
    public event EventHandler Exit;
    
    private Point _startPoint;

    public MainView()
    {
        InitializeComponent();
        
        Move.PointerPressed += OnPointerPressed;
        Move.PointerReleased += OnPointerReleased;
        Move.PointerMoved += OnPointerMoved;
    }

    private void Button_OnClickExit(object? sender, RoutedEventArgs e)
    {
        Exit?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(this);
        e.Pointer.Capture(this);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }
        
        var currentPosition = e.GetPosition(this);
        var deltaX = currentPosition.X - _startPoint.X;
        var deltaY = currentPosition.Y - _startPoint.Y;

        var lifeTime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifeTime == null) return;
        if (lifeTime.MainWindow == null) return;
        
        lifeTime.MainWindow.Position = new PixelPoint((int)(lifeTime.MainWindow.Position.X + deltaX), (int)(lifeTime.MainWindow.Position.Y + deltaY));
    }
}
