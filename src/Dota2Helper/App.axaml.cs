using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Dota2Helper.Design;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Gsi;
using Dota2Helper.Features.Http;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.Features.Timers;
using Dota2Helper.ViewModels;
using Dota2Helper.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static Avalonia.Controls.Design;

namespace Dota2Helper;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider => _host?.Services ?? throw new InvalidOperationException("Service provider is not initialized");
    static IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();

        builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                if (!IsDesignMode)
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("appsettings.timers.default.json", optional: false, reloadOnChange: false);
                }
            }
        );

        builder.ConfigureServices((hostingContext, services) =>
            {
                services.AddOptions();

                if (IsDesignMode)
                {
                    services.AddSingleton<SettingsService>(new DesignSettingsService());
                }
                else
                {
                    services
                        .Configure<Settings>(hostingContext.Configuration)
                        .Configure<List<DotaTimer>>(hostingContext.Configuration.GetSection("DefaultTimers").Bind);

                    services
                        .AddSingleton<SettingsService>();
                }

                services
                    .AddSingleton<GameTimeProvider>()
                    .AddSingleton<ITimeProvider>(sp => sp.GetRequiredService<GameTimeProvider>())
                    .AddSingleton<DemoTimeProvider>()
                    .AddSingleton<ViewModelFactory>()
                    .AddKeyedSingleton<IAudioService, AudioService>(nameof(AudioService))
                    .AddKeyedSingleton<IAudioService, TimerAudioService>(nameof(TimerAudioService))
                    .AddSingleton<RealGameTimeProvider>()
                    .AddSingleton<GsiConfigWatcher>()
                    .AddSingleton<GsiConfigService>()
                    .AddSingleton<ProfileService>()
                    .AddSingleton<AudioService>()
                    .AddSingleton<TimerAudioService>()
                    .AddSingleton<SettingsWindow>()
                    .AddSingleton<LocalListener>()
                    .AddSingleton<SplashScreenViewModel>()
                    .AddSingleton<TimersViewModel>()
                    .AddSingleton<SettingsViewModel>();

                services
                    .AddHostedService(sp => sp.GetRequiredService<GameTimeProvider>())
                    .AddHostedService(sp => sp.GetRequiredService<LocalListener>())
                    .AddHostedService(sp => sp.GetRequiredService<DemoTimeProvider>());
            }
        );

        _host = builder.Build();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splash = new SplashWindow(dataContext: ServiceProvider.GetRequiredService<SplashScreenViewModel>());
            desktop.MainWindow = splash;
            splash.Show();

            await Task.WhenAny(
                Task.Delay(8000),
                Task.Delay(Timeout.Infinite, ((SplashScreenViewModel)splash.DataContext!).CancellationToken)
            );

            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.MainWindow = new TimersWindow()
            {
                DataContext = ServiceProvider.GetRequiredService<TimersViewModel>(),
            };

            var task = _host.RunAsync();

            desktop.MainWindow.Show();
            splash.Close();

            desktop.MainWindow.Closing += (sender, args) =>
            {
                _host.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}