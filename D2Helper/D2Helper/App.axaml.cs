using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using D2Helper.Services;
using D2Helper.ViewModels;
using D2Helper.Views;

namespace D2Helper;

public partial class App : Application
{
    TimerService _timerService;
    GameStateService _gameStateService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _timerService = new TimerService();
        _gameStateService = new GameStateService();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.MainWindow = new TimersWindow()
            {
                DataContext = new TimersViewModel(_timerService, _gameStateService),
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new TimersWindow
            {
                DataContext = new TimersViewModel(_timerService, _gameStateService),
            };
        }

        _gameStateService.RunWorkerAsync();

        base.OnFrameworkInitializationCompleted();
    }
}