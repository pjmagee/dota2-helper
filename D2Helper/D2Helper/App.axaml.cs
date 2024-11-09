using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using D2Helper.Features.Audio;
using D2Helper.Features.Gsi;
using D2Helper.Features.Http;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;
using D2Helper.Views;
using Microsoft.Extensions.DependencyInjection;

namespace D2Helper;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Service provider is not initialized");

    static IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection()
            .AddSingleton<StrategyTimeProvider>()
            .AddSingleton<SettingsService>()
            .AddSingleton<ITimeProviderStrategy>(sp => sp.GetRequiredService<StrategyTimeProvider>())
            .AddSingleton<IGameTimeProvider>(sp => sp.GetRequiredService<StrategyTimeProvider>())
            .AddSingleton<DemoTimeProvider>()
            .AddSingleton<GameTimeProvider>()
            .AddSingleton<GsiConfigWatcher>()
            .AddSingleton<GsiConfigService>()
            .AddSingleton<LocalListener>()
            .AddSingleton<TimerService>()
            .AddSingleton<AudioService>()
            .AddSingleton<TimersViewModel>()
            .AddSingleton<SettingsViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.MainWindow = new TimersWindow()
            {
                DataContext = ServiceProvider.GetRequiredService<TimersViewModel>(),
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new TimersWindow
            {
                DataContext = ServiceProvider.GetRequiredService<TimersViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}