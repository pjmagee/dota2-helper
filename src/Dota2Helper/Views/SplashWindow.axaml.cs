using Avalonia.Controls;

namespace Dota2Helper.Views;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public SplashWindow(object? dataContext) : this()
    {
        DataContext = dataContext;
    }
}