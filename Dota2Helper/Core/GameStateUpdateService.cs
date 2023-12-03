using System;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.ViewModels;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Core;

public class GameStateUpdateService : BackgroundService
{
    private readonly GameStateHolder _container;
    private readonly MainViewModel _mainViewModel;
    private readonly IDotaListener _listener;
    
    public GameStateUpdateService(GameStateHolder container, MainViewModel mainViewModel, IDotaListener listener)
    {
        _container = container;
        _mainViewModel = mainViewModel;
        _listener = listener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _container.State = await _listener.GetStateAsync();
                _mainViewModel.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}