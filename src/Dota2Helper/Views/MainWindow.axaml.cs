using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

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

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            window.HideMinimizeAndMaximizeButtons();
        }
    }
}
