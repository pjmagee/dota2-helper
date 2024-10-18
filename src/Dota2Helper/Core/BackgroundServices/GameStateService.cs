using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Listeners;
using Dota2Helper.Core.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dota2Helper.Core.BackgroundServices;

public class GameStateService(ILogger<GameStateService> logger, GameStateHolder container, DotaTimers timers, IListenerStrategy listenerStrategy) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Factory.StartNew((ct) => UpdateTimers((CancellationToken)ct!), stoppingToken, TaskCreationOptions.LongRunning);
    }

    async Task UpdateTimers(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                container.State = await listenerStrategy.Listener.GetStateAsync(stoppingToken);
                TimeSpan gameTime = container.State?.GameTime.GetValueOrDefault() ?? TimeSpan.FromSeconds(-30);

                await Dispatcher.UIThread.InvokeAsync(() => timers.Do(timer =>
                        {
                            try
                            {
                                timer.Update(gameTime);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Error updating timer");
                            }
                        }
                    ), DispatcherPriority.Background, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating timers");
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
    }
}