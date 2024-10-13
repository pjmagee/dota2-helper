using System;
using Avalonia.Controls;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        var window = (Window)sender!;
        window.HideMinimizeAndMaximizeButtons();
    }
}
