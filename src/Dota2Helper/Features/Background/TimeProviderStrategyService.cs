using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.TimeProvider;
using Dota2Helper.Features.Timers;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Features.Background;

public class TimeProviderStrategyService(
    SettingsService settingsService,
    RealGameTimeProvider realGameTimeProvider,
    DemoGameTimeProvider demoGameTimeProvider,
    GameTimeProvider gameTimeProvider) : BackgroundService
{
    Process? _dota2;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(settingsService.Settings.Mode is TimeMode.Real)
            {
                gameTimeProvider.Current = realGameTimeProvider;
            }
            else if (settingsService.Settings.Mode is TimeMode.Demo)
            {
                gameTimeProvider.Current = demoGameTimeProvider;
            }
            else if (settingsService.Settings.Mode is TimeMode.Auto)
            {
                if (_dota2 is null)
                {
                    _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

                    if (_dota2 is null)
                    {
                        gameTimeProvider.Current = demoGameTimeProvider;
                    }
                    else
                    {
                        gameTimeProvider.Current = realGameTimeProvider;

                        _dota2.EnableRaisingEvents = true;
                        _dota2.Exited += (_, _) =>
                        {
                            _dota2 = null;
                        };
                    }
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}