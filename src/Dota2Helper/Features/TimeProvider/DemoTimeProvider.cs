using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Features.TimeProvider;

public class DemoTimeProvider : BackgroundService, ITimeProvider
{
    public ProviderType ProviderType => ProviderType.Demo;
    public TimeSpan Time { get; private set; } = TimeSpan.FromSeconds(-90);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Time > TimeSpan.FromMinutes(60))
            {
                Time = TimeSpan.FromSeconds(-90);
            }

            Time += TimeSpan.FromSeconds(1);
            await Task.Delay(500, stoppingToken);
        }
    }
}