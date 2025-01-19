using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Logging;
using Dota2Helper.Features.TimeProvider;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Features.Background;

public class DemoGameTimeProvider : BackgroundService, ITimeProvider
{
    public TimeProviderType TimeProviderType => TimeProviderType.Demo;
    public TimeSpan Time { get; private set; } = TimeSpan.FromSeconds(-90);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (Time > TimeSpan.FromMinutes(60))
                {
                    Time = TimeSpan.FromSeconds(-90);
                }

                Time += TimeSpan.FromSeconds(1);
                await Task.Delay(400, stoppingToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}