using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Features.TimeProvider;

public class GameTimeProvider : BackgroundWorker, ITimeProvider
{
    public ProviderType ProviderType => _current?.ProviderType ?? ProviderType.Real;
    public TimeSpan Time => _current?.Time ?? TimeSpan.Zero;

    Process? _dota2;
    ITimeProvider? _current;
    readonly SettingsService _settingsService;
    readonly RealProvider _realProvider;
    readonly DemoProvider _demoProvider;

    public GameTimeProvider(SettingsService settingsService, RealProvider realProvider, DemoProvider demoProvider)
    {
        _settingsService = settingsService;
        _realProvider = realProvider;
        _demoProvider = demoProvider;
        RunWorkerAsync();
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            if(_settingsService.Settings.Mode is TimeMode.Real)
            {
                _current = _realProvider;
            }
            else if (_settingsService.Settings.Mode is TimeMode.Demo)
            {
                _current = _demoProvider;
            }
            else if (_settingsService.Settings.Mode is TimeMode.Auto)
            {
                if (_dota2 is null)
                {
                    _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

                    if (_dota2 is null)
                    {
                        _current = _demoProvider;
                    }
                    else
                    {
                        _current = _realProvider;
                        _dota2.EnableRaisingEvents = true;
                        _dota2.Exited += (sender, args) =>
                        {
                            _dota2 = null;
                            _current = _demoProvider;
                        };
                    }
                }
            }

            Thread.Sleep(2000);
        }
    }
}