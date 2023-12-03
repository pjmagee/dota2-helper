using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Dota2Helper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainView_OnExit(object? sender, EventArgs e)
    {
        Close();
    }
}
