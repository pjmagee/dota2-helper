using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using Avalonia.Threading;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Timers;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class TimersViewModel : ViewModelBase
{
    readonly ILogger<TimersViewModel> _logger;
    readonly GameStateHolder _stateHolder;
    readonly AudioPlayer _audioPlayer;

    readonly static JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public DotaTimers Timers { get; }

    public TimersViewModel(
        ILogger<TimersViewModel> logger, 
        DotaTimers timers, 
        GameStateHolder stateHolder, 
        AudioPlayer audioPlayer)
    {
        _logger = logger;
        _stateHolder = stateHolder;
        _audioPlayer = audioPlayer;
        Timers = timers;
        WireEvents();
    }
    
    ~TimersViewModel() => UnWireEvents();

    void WireEvents()
    {
        foreach (var timer in Timers)
        {
            timer.OnReminder -= QueueReminder;
            timer.OnReminder += QueueReminder;
        }
    }

    void QueueReminder(object? sender, EventArgs args)
    {
        var timer = (DotaTimer) sender!;
        _audioPlayer.QueueReminder(timer);
    }

    void UnWireEvents()
    {
        foreach (var timer in Timers)
        {
            timer.OnReminder -= QueueReminder;
        }
    }
}