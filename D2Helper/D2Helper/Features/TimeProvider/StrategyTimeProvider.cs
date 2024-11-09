using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using D2Helper.Features.Timers;

namespace D2Helper.Features.TimeProvider;

public class StrategyTimeProvider : BackgroundWorker, IGameTimeProvider, ITimeProviderStrategy
{
    public StrategyTimeProvider(GameTimeProvider gameTimeProvider, DemoTimeProvider demoTimeProvider)
    {
        _gameTimeProvider = gameTimeProvider;
        _demoTimeProvider = demoTimeProvider;

        RunWorkerAsync();
    }

    public TimeProviderStrategy Strategy
    {
        get => _strategy ?? TimeProviderStrategy.Auto;
        set
        {
            _strategy = value;
            _current = _strategy switch
            {
                TimeProviderStrategy.Real => _gameTimeProvider,
                TimeProviderStrategy.Demo => _demoTimeProvider,
                TimeProviderStrategy.Auto => null,
                _ => _current
            };
        }
    }

    Process? _dota2;
    IGameTimeProvider? _current;
    TimeProviderStrategy? _strategy;
    readonly GameTimeProvider _gameTimeProvider;
    readonly DemoTimeProvider _demoTimeProvider;

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            if (Strategy is TimeProviderStrategy.Auto)
            {
                if (_dota2 == null || _dota2.HasExited)
                {
                    _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

                    if(_dota2 != null)
                    {
                        _dota2.Exited += (sender, args) =>
                        {
                            Strategy = TimeProviderStrategy.Demo;
                            _dota2 = null;
                        };
                    }
                }

                Strategy = _dota2 != null ? TimeProviderStrategy.Real : TimeProviderStrategy.Demo;
            }

            Thread.Sleep(2000);
        }
    }

    public TimeSpan Time => _current?.Time ?? TimeSpan.Zero;
}