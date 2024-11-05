using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using D2Helper.Services;
using D2Helper.ViewModels;
using D2Helper.Views;
using Microsoft.Extensions.DependencyInjection;

namespace D2Helper;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Service provider is not initialized");

    private static IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<DynamicProvider>()
            .AddSingleton<IStrategyProvider>(sp => sp.GetRequiredService<DynamicProvider>())
            .AddSingleton<IGameTimeProvider>(sp => sp.GetRequiredService<DynamicProvider>())
            .AddSingleton<DemoGameTimeProvider>()
            .AddSingleton<RealGameTimeProvider>()
            .AddSingleton<Dota2ConfigurationService>()
            .AddSingleton<LongLivedHttpListener>()
            .AddSingleton<TimerService>()
            .AddSingleton<TimersViewModel>()
            .AddSingleton<SettingsViewModel>()
            .BuildServiceProvider();

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