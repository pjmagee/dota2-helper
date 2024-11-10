using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using D2Helper.Design;
using D2Helper.Features.Audio;
using D2Helper.Features.Gsi;
using D2Helper.Features.Http;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.ViewModels;
using D2Helper.Views;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = D2Helper.Features.TimeProvider.TimeProvider;

namespace D2Helper;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Service provider is not initialized");

    static IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection()
            .AddSingleton<TimeProvider>()
            .AddSingleton<SettingsService>()
            .AddSingleton<ITimeProvider>(sp => sp.GetRequiredService<TimeProvider>())

            .AddSingleton<DemoProvider>()
            .AddSingleton<RealProvider>()

            .AddSingleton<GsiConfigWatcher>()
            .AddSingleton<GsiConfigService>()

            .AddSingleton<TimerService>()
            .AddSingleton<AudioService>()
            .AddSingleton<TimerAudioService>()

            .AddSingleton<LocalListener>()
            .AddSingleton<SplashScreenViewModel>()
            .AddSingleton<TimersViewModel>()
            .AddSingleton<SettingsViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        var localListener = ServiceProvider.GetRequiredService<LocalListener>();
        localListener.RunWorkerAsync();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splash = new SplashWindow()
            {
                DataContext = ServiceProvider.GetRequiredService<SplashScreenViewModel>(),
            };

            desktop.MainWindow = splash;
            splash.Show();

            await Task.Delay(5000);



            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.MainWindow = new TimersWindow()
            {
                DataContext = ServiceProvider.GetRequiredService<TimersViewModel>(),
            };

            desktop.MainWindow.Show();
            splash.Close();
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