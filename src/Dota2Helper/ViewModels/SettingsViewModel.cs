using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Gsi;
using Dota2Helper.Core.Timers;
using DynamicData.Binding;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    readonly static object WriterLock = new();


    readonly SteamLibraryService _steamLibraryService;
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

    public SettingsViewModel(SteamLibraryService steamLibraryService, AudioPlayer audioPlayer, DotaTimers timers)
    {
        _steamLibraryService = steamLibraryService;
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
        _steamLibraryService.InstallIntegration();
        this.RaisePropertyChanged(nameof(IsIntegrated));
    }

    // Open folder with steam dota2 install
    public void Open()
    {
        _steamLibraryService.OpenGameStateIntegrationFolder();
    }

    public bool IsIntegrated => _steamLibraryService.IsIntegrationInstalled();

    public void ToggleTheme()
    {
        var toggle = Application.Current!.RequestedThemeVariant switch
        {
            { Key: nameof(ThemeVariant.Light) }  => ThemeVariant.Dark,
            { Key: nameof(ThemeVariant.Dark) } => ThemeVariant.Light,
            _ => null,
        };

        Application.Current!.RequestedThemeVariant = toggle;
        ThemeName = (toggle == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark).Key.ToString();
    }

    string? _themeName;
    public string? ThemeName
    {
        get => _themeName;
        set => this.RaiseAndSetIfChanged(ref _themeName, value);
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
                            // Update the properties of the timer object
                            item.First = timer.First;
                            item.Interval = timer.Interval;
                            item.Reminder = timer.Reminder;
                            item.AudioFile = timer.AudioFile;
                            item.IsManualReset = timer.IsManualReset;
                            item.IsEnabled = timer.IsEnabled;
                            item.IsTts = timer.IsTts;
                            break;
                        }
                    }
                    
                    File.WriteAllText("appsettings.json", JsonSerializer.Serialize(appSettings, JsonContext.Default.Options));
                }
            }
        }
    }
}