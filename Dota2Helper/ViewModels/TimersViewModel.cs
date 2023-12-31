﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Avalonia.Threading;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Timers;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class TimersViewModel : ViewModelBase
{
    private readonly ILogger<TimersViewModel> _logger;
    private readonly GameStateHolder _stateHolder;
    private readonly AudioPlayer _audioPlayer;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    public bool IsSpeakerMuted => Volume <= 0;
    
    public bool IsSpeakerOn => Volume > 0;
    
    private ObservableCollection<DotaTimer>? _timers;

    public ObservableCollection<DotaTimer>? Timers
    {
        get => _timers;
        set
        {
            _timers = value;
            this.RaisePropertyChanged(nameof(Timers));
        }
    }
    
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


    public TimersViewModel(ILogger<TimersViewModel> logger, DotaTimers timers, GameStateHolder stateHolder, AudioPlayer audioPlayer)
    {
        _logger = logger;
        _stateHolder = stateHolder;
        _audioPlayer = audioPlayer;
        Timers = timers;
        
        WireEvents();
    }
    
    ~TimersViewModel() => UnWireEvents();

    private void WireEvents()
    {
        foreach (var timer in Timers!)
        {
            timer.OnReminder += QueueReminder;
        }
    }
    
    private void QueueReminder(object? sender, EventArgs args)
    {
        var timer = (DotaTimer) sender!;
        _audioPlayer.QueueReminder(timer);
    }

    private void UnWireEvents()
    {
        foreach (var timer in Timers!)
        {
            timer.OnReminder -= QueueReminder;
        }
    }

    public void Update()
    {
        if (_stateHolder.State is not null)
        {
            _logger.LogInformation("{Json}", JsonSerializer.Serialize(_stateHolder.State.Map, Options));

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
                    for(int i = 0; i < Timers.Count; i++)
                    {
                        var timer = Timers[i];
                        timer.Update(time);
                        Timers.Move(i, i);
                    }
                });
            }
        }
    }
}