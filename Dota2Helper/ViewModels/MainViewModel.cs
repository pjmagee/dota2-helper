using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using Avalonia.Styling;
using Avalonia.Threading;
using Dota2Helper.Core;
using Microsoft.Extensions.Logging;
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
    
    public void Theme()
    {
        var toggle = App.Current.RequestedThemeVariant switch
        {
            { Key: nameof(ThemeVariant.Light) }  => ThemeVariant.Dark,
            { Key: nameof(ThemeVariant.Dark) } => ThemeVariant.Light,
            _ => null,
        };
        
        App.Current.RequestedThemeVariant = toggle;

        ThemeName = (toggle == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark).Key.ToString();
    }

    private string? _themeName;

    public string ThemeName
    {
        get
        {
            if (_themeName is null)
            {
                _themeName = App.Current.ActualThemeVariant switch
                {
                    { Key: nameof(ThemeVariant.Light) } => ThemeVariant.Dark.Key.ToString(),
                    { Key: nameof(ThemeVariant.Dark) } => ThemeVariant.Light.Key.ToString(),
                    _ => "Unknown",
                };
            }

            return _themeName!;
        }
        private set
        {
            this.RaiseAndSetIfChanged(ref _themeName, value);
        }
    }

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
    
    private ObservableCollection<DotaTimer> _timers;

    public ObservableCollection<DotaTimer> Timers
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


    public MainViewModel(ILogger<MainViewModel> logger, ObservableCollection<DotaTimer> timers, GameStateHolder stateHolder, AudioPlayer audioPlayer)
    {
        _logger = logger;
        _stateHolder = stateHolder;
        _audioPlayer = audioPlayer;
        Timers = timers;
        
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