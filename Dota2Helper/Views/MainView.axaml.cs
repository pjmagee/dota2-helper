using System;
using Avalonia;
using Avalonia.Controls;

namespace Dota2Helper.Views;

public partial class MainView : UserControl
{
    public event EventHandler Exit;
    
    private Point _startPoint;
    
    public string ThemeName { get; set; }

    public void Theme()
    {
        
    }

    public MainView()
    {
        InitializeComponent();
    }
}
