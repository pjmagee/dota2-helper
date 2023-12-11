using System;
using Avalonia.Controls;
using Dota2Helper.Core;

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
