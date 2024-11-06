using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using D2Helper.Models;

namespace D2Helper.Services;

public class DynamicProvider : BackgroundWorker, IGameTimeProvider, IStrategyProvider
{
    public DynamicProvider(RealGameTimeProvider gameTimeProvider, DemoGameTimeProvider demoGameTimeProvider)
    {
        _gameTimeProvider = gameTimeProvider;
        _demoGameTimeProvider = demoGameTimeProvider;

        RunWorkerAsync();
    }

    public GameStateStrategy Strategy
    {
        get => _strategy ?? GameStateStrategy.Auto;
        set
        {
            _strategy = value;
            _current = _strategy switch
            {
                GameStateStrategy.Real => _gameTimeProvider,
                GameStateStrategy.Demo => _demoGameTimeProvider,
                GameStateStrategy.Auto => null,
                _ => _current
            };
        }
    }

    Process? _dota2;
    IGameTimeProvider? _current;
    GameStateStrategy? _strategy;
    readonly RealGameTimeProvider _gameTimeProvider;
    readonly DemoGameTimeProvider _demoGameTimeProvider;

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            if (Strategy is GameStateStrategy.Auto)
            {
                if (_dota2 == null || _dota2.HasExited)
                {
                    _dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();

                    if(_dota2 != null)
                    {
                        _dota2.Exited += (sender, args) =>
                        {
                            Strategy = GameStateStrategy.Demo;
                            _dota2 = null;
                        };
                    }
                }

                Strategy = _dota2 != null ? GameStateStrategy.Real : GameStateStrategy.Demo;
            }

            Thread.Sleep(2000);
        }
    }

    public TimeSpan Time => _current?.Time ?? TimeSpan.Zero;
}