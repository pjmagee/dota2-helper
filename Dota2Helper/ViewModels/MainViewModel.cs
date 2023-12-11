using System;
using System.Text.Json;
using Avalonia.Media;
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
    private readonly AudioPlayer _audioPlayer;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private double _opacity = 0.5d;

    public double Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Round(value, 1);
            this.RaisePropertyChanged();
        }
    }

    public bool IsSpeakerMuted => Volume <= 0;
    
    public bool IsSpeakerOn => Volume > 0;
    
    public DotaTimers Timers { get; set; }
    
    public double Volume
    {
        set
        {
            _audioPlayer.Volume = (int) value;
            this.RaisePropertyChanged(nameof(IsSpeakerMuted));
            this.RaisePropertyChanged(nameof(IsSpeakerOn));
        }
        get => _audioPlayer.Volume;
    }


    public MainViewModel(ILogger<MainViewModel> logger, DotaTimers dotaTimers, GameStateHolder stateHolder, AudioPlayer audioPlayer)
    {
        _logger = logger;
        _stateHolder = stateHolder;
        _audioPlayer = audioPlayer;
        Timers = dotaTimers;
        
        WireEvents();
    }
    
    ~MainViewModel() => UnWireEvents();

    private void WireEvents()
    {
        foreach (var timer in Timers)
        {
            timer.OnReminder += QueueReminder;
        }
    }
    
    private void QueueReminder(object? sender, EventArgs args)
    {
        var timer = (DotaTimer) sender!;
        _audioPlayer.QueueReminder(timer!.SoundToPlay);
    }

    private void UnWireEvents()
    {
        foreach (var timer in Timers)
        {
            timer.OnReminder -= QueueReminder;
        }
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