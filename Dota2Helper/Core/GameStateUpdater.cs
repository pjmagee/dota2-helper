using System;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Core;

public class GameStateUpdater(GameStateHolder container, MainViewModel mainViewModel, IDotaListener listener) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                container.State = await listener.GetStateAsync();
                mainViewModel.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}