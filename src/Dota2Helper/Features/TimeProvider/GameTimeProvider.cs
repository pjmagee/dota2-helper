using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;
using Microsoft.Extensions.Hosting;

namespace Dota2Helper.Features.TimeProvider;

public class GameTimeProvider(SettingsService settingsService, RealGameTimeProvider realGameTimeProvider, DemoTimeProvider demoTimeProvider) : BackgroundService, ITimeProvider
{
    public ProviderType ProviderType => _current?.ProviderType ?? ProviderType.Real;
    public TimeSpan Time => _current?.Time ?? TimeSpan.Zero;

    Process? _dota2;
    ITimeProvider? _current;
    private readonly SemaphoreSlim _signal = new(0);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(settingsService.Settings.Mode is TimeMode.Real)
            {
                _current = realGameTimeProvider;
            }
            else if (settingsService.Settings.Mode is TimeMode.Demo)
            {
                _current = demoTimeProvider;
            }
            else if (settingsService.Settings.Mode is TimeMode.Auto)
            {
                if (_dota2 is null)
                {
                    _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

                    if (_dota2 is null)
                    {
                        _current = demoTimeProvider;
                    }
                    else
                    {
                        _current = realGameTimeProvider;
                        _dota2.EnableRaisingEvents = true;
                        _dota2.Exited += (sender, args) =>
                        {
                            _dota2 = null;
                            _current = demoTimeProvider;
                            _signal.Release();
                        };
                    }
                }
            }

            await _signal.WaitAsync(2000, stoppingToken);
        }
    }
}