using System;
using System.Text.Json;
using Avalonia.Threading;

using Dota2Helper.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly GameStateHolder _stateHolder;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public bool IsSpeakerMuted => Volume <= 0;
    
    public bool IsSpeakerOn => Volume > 0;
    
    public DotaTimers Timers { get; set; }
    
    public double Volume
    {
        set
        {
            foreach (var timer in Timers)
            {
                timer.Volume = value;
            }
            
            this.RaisePropertyChanged(nameof(IsSpeakerMuted));
            this.RaisePropertyChanged(nameof(IsSpeakerOn));
        }
        get => Timers[0].Volume;
    }


    public MainViewModel(ILogger<MainViewModel> logger, GameStateHolder stateHolder)
    {
        _logger = logger;
        _stateHolder = stateHolder;
        Timers = new DotaTimers();
    }

    public MainViewModel() : this(new NullLogger<MainViewModel>(), new GameStateHolder())
    {
       
    }

    public void Update()
    {
        if (_stateHolder?.State is not null)
        {
            _logger.LogInformation(JsonSerializer.Serialize(_stateHolder.State.Map, Options));
            
            TimeSpan time = TimeSpan.FromSeconds(_stateHolder.State.Map.ClockTime);

            var updateState = _stateHolder.State.Map.GameState switch
            {
                "DOTA_GAMERULES_STATE_PRE_GAME" => true,
                "DOTA_GAMERULES_STATE_GAME_IN_PROGRESS" => true,
                _ => false,
            };
                                                        
            if (updateState)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    for (int i = 0; i < Timers.Count; i++)
                    {

                        Timers[i].Update(time);
                        Timers.Update(i, Timers[i]);
                    }
                });
            }
        }
    }
}