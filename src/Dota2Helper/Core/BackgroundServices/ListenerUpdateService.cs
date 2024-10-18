using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Dota2Helper.Core.Listeners;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.BackgroundServices;

public class ListenerUpdateService(ILogger<ListenerUpdateService> logger, IListenerStrategy listenerStrategy, SettingsViewModel settingsViewModel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Factory.StartNew((ct) => UpdateListener((CancellationToken)ct!), stoppingToken, TaskCreationOptions.LongRunning);
    }

    async Task UpdateListener(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                listenerStrategy.UpdateListener();

                Dispatcher.UIThread.Post(() =>
                {
                    settingsViewModel.IsDotaListener = listenerStrategy.Listener is DotaListener;
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating listener");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}