using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
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
using static Avalonia.Controls.Design;

namespace Dota2Helper;

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
        var services = new ServiceCollection().AddOptions();

        if (IsDesignMode)
        {
            services
                .Configure<Settings>(o =>
                {
                        o.Mode = TimeMode.Auto;
                        o.Volume = 50;
                        o.Theme = "Light";
                        o.DemoMuted = true;
                        o.SelectedProfileIdx = 0;
                        o.Profiles = new List<Profile>
                        {
                            new Profile
                            {
                                Name = "Default",
                                Timers = new List<DotaTimer>
                                {
                                    new DotaTimer
                                    {
                                        Name = "Roshan",
                                        Time = TimeSpan.FromMinutes(8),
                                        IsInterval = false,
                                        IsManualReset = true,
                                        IsEnabled = true,
                                        IsMuted = false,
                                        RemindAt = TimeSpan.FromMinutes(1),
                                        StopAfter = TimeSpan.FromMinutes(1),
                                        StartAfter = TimeSpan.FromMinutes(1),
                                        AudioFile = null,
                                    }
                                }
                            }
                        };
                })
                .Configure<List<DotaTimer>>(o =>
                {
                    o.Add(new DotaTimer
                    {
                        Name = "Roshan",
                        Time = TimeSpan.FromMinutes(8),
                        IsInterval = false,
                        IsManualReset = true,
                        IsEnabled = true,
                        IsMuted = false,
                        RemindAt = TimeSpan.FromMinutes(1),
                        StopAfter = TimeSpan.FromMinutes(1),
                        StartAfter = TimeSpan.FromMinutes(1),
                        AudioFile = null,
                    });
                });
        }
        else
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.timers.default.json", optional: false, reloadOnChange: false)
                .Build();

            services
                .Configure<Settings>(configuration.Bind)
                .Configure<List<DotaTimer>>(configuration.GetSection("DefaultTimers").Bind);
        }

        services
            .AddSingleton<SettingsService>()
            .AddSingleton<GameTimeProvider>()
            .AddSingleton<ITimeProvider>(sp => sp.GetRequiredService<GameTimeProvider>())
            .AddSingleton<DemoProvider>()
            .AddSingleton<ViewModelFactory>()
            .AddKeyedSingleton<IAudioService, AudioService>(nameof(AudioService))
            .AddKeyedSingleton<IAudioService, TimerAudioService>(nameof(TimerAudioService))
            .AddSingleton<RealProvider>()
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

        _serviceProvider = services.BuildServiceProvider();

        var dota2ConfigService = ServiceProvider.GetRequiredService<GsiConfigService>();
        var localListener = ServiceProvider.GetRequiredService<LocalListener>();
        localListener.RunWorkerAsync();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splash = new SplashWindow
            {
                DataContext = ServiceProvider.GetRequiredService<SplashScreenViewModel>(),
            };

            desktop.MainWindow = splash;
            splash.Show();

            dota2ConfigService.TryInstall();

            await Task.WhenAny(
                Task.Delay(8000),
                Task.Delay(Timeout.Infinite, ((SplashScreenViewModel)splash.DataContext).CancellationToken)
            );

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