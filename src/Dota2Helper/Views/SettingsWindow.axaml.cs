using Avalonia.Controls;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    public SettingsWindow(SettingsViewModel settingsViewModel) : this()
    {
        DataContext = settingsViewModel;
    }

    void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}