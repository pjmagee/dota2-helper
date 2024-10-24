using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Timers;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    readonly static object WriterLock = new();

    readonly GameStateService _gameStateService;
    readonly AudioPlayer _audioPlayer;

    public DotaTimers Timers { get; }

    public double Volume
    {
        set
        {
            _audioPlayer.Volume = (int)value;
            this.RaisePropertyChanged(nameof(IsSpeakerMuted));
            this.RaisePropertyChanged(nameof(IsSpeakerOn));
        }
        get => _audioPlayer.Volume;
    }

    public SettingsViewModel(GameStateService gameStateService, AudioPlayer audioPlayer, DotaTimers timers)
    {
        _gameStateService = gameStateService;
        _audioPlayer = audioPlayer;
        Timers = timers;

        ThemeName = Application.Current!.ActualThemeVariant.Key.ToString();

        foreach (var dotaTimer in Timers)
        {
            dotaTimer.PropertyChanged -= TimerOnPropertyChanged;
            dotaTimer.PropertyChanged += TimerOnPropertyChanged;
        }
    }

    public bool IsSpeakerMuted => Volume <= 0;

    public bool IsSpeakerOn => Volume > 0;

    ImmutableArray<string> Properties =>
    [
        nameof(DotaTimer.IsEnabled),
        nameof(DotaTimer.IsSoundEnabled),
        nameof(DotaTimer.IsTts),
        nameof(DotaTimer.Reminder)
    ];

    public void Install()
    {
        _gameStateService.InstallIntegration();
        this.RaisePropertyChanged(nameof(IsIntegrated));
    }

    // Open folder with steam dota2 install
    public void Open()
    {
        _gameStateService.OpenGameStateIntegrationFolder();
    }

    public bool IsIntegrated => _gameStateService.IsIntegrationInstalled();

    public void ToggleTheme()
    {
        var toggle = Application.Current!.RequestedThemeVariant switch
        {
            { Key: nameof(ThemeVariant.Light) } => ThemeVariant.Dark,
            { Key: nameof(ThemeVariant.Dark) } => ThemeVariant.Light,
            _ => null,
        };

        Application.Current!.RequestedThemeVariant = toggle;
        ThemeName = (toggle == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark).Key.ToString();
    }

    int? _portNumber;

    public int? PortNumber
    {
        get => _gameStateService.GetPortNumber();
        set
        {
            if (value != _portNumber)
            {
                _gameStateService.SetPortNumber(value!.Value);
            }

            this.RaiseAndSetIfChanged(ref _portNumber, value);
        }
    }

    string? _themeName;

    public string? ThemeName
    {
        get => _themeName;
        set => this.RaiseAndSetIfChanged(ref _themeName, value);
    }

    private bool _isDotaListener;

    public bool IsDotaListener
    {
        get => _isDotaListener;
        set => this.RaiseAndSetIfChanged(ref _isDotaListener, value);
    }

    void TimerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update the appsettings.json file when a valid timer property changes
        if (sender is DotaTimer timer && e.PropertyName is not null && Properties.Contains(e.PropertyName))
        {
            lock (WriterLock)
            {
                using (var doc = JsonDocument.Parse(File.ReadAllText("appsettings.json")))
                {
                    var appSettings = doc.RootElement.Deserialize<AppSettings>(JsonContext.Default.Options)!;

                    foreach (var item in appSettings.Settings.Timers)
                    {
                        if (item.Label == timer.Label)
                        {
                            item.First = timer.First;
                            item.Interval = timer.Interval;
                            item.Reminder = timer.Reminder;
                            item.AudioFile = timer.AudioFile;
                            item.IsManualReset = timer.IsManualReset;
                            item.IsSoundEnabled = timer.IsSoundEnabled;
                            item.IsEnabled = timer.IsEnabled;
                            item.IsTts = timer.IsTts;
                            break;
                        }
                    }

                    File.WriteAllText("appsettings.json", JsonSerializer.Serialize(appSettings, JsonContext.Custom.Options));
                }
            }
        }
    }
}