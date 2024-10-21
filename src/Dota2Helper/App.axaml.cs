using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dota2Helper.ViewModels;
using Dota2Helper.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.BackgroundServices;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Framework;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Listeners;
using Dota2Helper.Core.Timers;
using Microsoft.Extensions.Logging;
using GameStateService = Dota2Helper.Core.Gsi.GameStateService;
using ViewLocator = Dota2Helper.Core.Framework.ViewLocator;
using Hosting = Microsoft.Extensions.Hosting;

namespace Dota2Helper;

public class App : Application
{
    public IHost? Host { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        Host = CreateHost();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Host.Services.GetRequiredService<MainWindowViewModel>(),
            };

            desktop.ShutdownRequested += (sender, args) =>
            {
                foreach (var listener in Host.Services.GetServices<IDotaListener>())
                {
                    try
                    {
                        listener.Dispose();
                    }
                    catch (Exception e)
                    {
                        Host.Services.GetRequiredService<ILogger<App>>().LogError(e, "Error disposing listener");
                    }
                }

                Host.Dispose();
                Host = null;
            };
        }

        DataTemplates.Add(Host.Services.GetRequiredService<ViewLocator>());

        base.OnFrameworkInitializationCompleted();
        await Host.StartAsync();
    }

    static IHost CreateHost()
    {
        HostApplicationBuilder builder = Hosting.Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());

        builder.Services.AddSingleton<FakeDotaListener>();
        builder.Services.AddSingleton<DotaListener>();
        builder.Services.AddSingleton<IListenerStrategy, DynamicListenerStrategy>();
        builder.Services.AddSingleton<GameStateService>();
        builder.Services.AddSingleton<IDotaListener>(serviceProvider => serviceProvider.GetRequiredService<DotaListener>());
        builder.Services.AddSingleton<IDotaListener>(serviceProvider => serviceProvider.GetRequiredService<FakeDotaListener>());

        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();

        builder.Services.Configure<Settings>(options =>
            {
                using (var stream = File.OpenRead("appsettings.json"))
                {
                    using (var document = JsonDocument.Parse(stream))
                    {
                        var parsed = document.RootElement.Deserialize<AppSettings>(JsonContext.Default.Options);

                        if (parsed != null)
                        {
                            options.Timers = parsed.Settings.Timers;
                        }
                    }
                }
            }
        );

        builder.Services.AddSingleton<DotaTimers>();
        builder.Services.AddSingleton<AudioPlayer>();
        builder.Services.AddSingleton<GameStateHolder>();

        builder.Services.AddTransient<ViewLocator>();

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<TimersViewModel>();

        builder.Services.AddView<MainWindowViewModel, MainWindow>();
        builder.Services.AddView<TimersViewModel, TimersView>();
        builder.Services.AddView<SettingsViewModel, SettingsView>();

        builder.Services.AddHostedService<Core.BackgroundServices.GameStateService>();
        builder.Services.AddHostedService<AudioPlayerService>();
        builder.Services.AddHostedService<ListenerUpdateService>();

        return builder.Build();
    }
}