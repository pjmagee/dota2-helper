using System;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Core.Listeners;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Core.Gsi;

public class GameStateUpdater(GameStateHolder container, TimersViewModel timersViewModel, IListenerStrategy listenerStrategy) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                container.State = await listenerStrategy.Current.GetStateAsync();
                timersViewModel.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}