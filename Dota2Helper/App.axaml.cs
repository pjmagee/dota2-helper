using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Dota2Helper.ViewModels;
using Dota2Helper.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Avalonia.Controls;
using Dota2Helper.Core;
using ViewLocator = Dota2Helper.Core.ViewLocator;

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
                DataContext = Host.Services.GetRequiredService<MainViewModel>()
            };
            
            desktop.Exit += async (sender, args) =>
            {
                var listener = Host.Services.GetRequiredService<IDotaListener>();
                listener.Dispose();
                
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

        builder.Services.AddHostedService<GameStateUpdateService>();


        if (Design.IsDesignMode)
        {
            builder.Services.AddSingleton<IDotaListener, FakeDotaListener>();
        }
        else if (Process.GetProcessesByName("dota2").Any())
        {
            builder.Services.AddSingleton<IDotaListener, DotaListener>();
        }
        else
        {
            builder.Services.AddSingleton<IDotaListener, FakeDotaListener>();
        }
        

        builder.Services.AddTransient<ViewLocator>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddView<MainViewModel, MainView>();
        builder.Services.AddSingleton<GameStateHolder>();

        return builder.Build();
    }
}