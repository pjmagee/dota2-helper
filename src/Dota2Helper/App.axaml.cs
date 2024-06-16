using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Dota2Helper.ViewModels;
using Dota2Helper.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Avalonia.Collections;
using Avalonia.Controls;
using Dota2Helper.Core;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Framework;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Listeners;
using Dota2Helper.Core.Timers;
using Microsoft.Extensions.Configuration;
using ViewLocator = Dota2Helper.Core.Framework.ViewLocator;

namespace Dota2Helper;

public partial class App : Application
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
                DataContext = Host.Services.GetRequiredService<MainWindowViewModel>()
            };
            
            desktop.Exit += async (sender, args) =>
            {
                foreach (var listener in Host.Services.GetRequiredService<List<IDotaListener>>())
                {
                    listener.Dispose();
                }
                
                await Host.StopAsync(TimeSpan.FromSeconds(5));
                Host.Dispose();
                Host = null;
            };
        }

        DataTemplates.Add(Host.Services.GetRequiredService<ViewLocator>());

        base.OnFrameworkInitializationCompleted();
        await Host.StartAsync();
    }

    private static IHost CreateHost()
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());

        builder.Services.AddSingleton<FakeDotaListener>();
        builder.Services.AddSingleton<DotaListener>();
        
        builder.Services.AddSingleton<List<IDotaListener>>(sp => [
            sp.GetRequiredService<FakeDotaListener>(), sp.GetRequiredService<DotaListener>()
        ]);
        
        builder.Services.AddSingleton<IListenerStrategy, DynamicListenerStrategy>();
        builder.Services.Configure<Settings>(options => builder.Configuration.GetSection("Settings").Bind(options));

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
        
        builder.Services.AddHostedService<GameStateUpdater>();
        builder.Services.AddHostedService<AudioPlayerService>();
        
        return builder.Build();
    }
}