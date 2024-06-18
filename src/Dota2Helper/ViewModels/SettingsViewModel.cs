using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Dota2Helper.Core.Audio;
using Dota2Helper.Core.Configuration;
using Dota2Helper.Core.Timers;
using ReactiveUI;

namespace Dota2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private static readonly object WriterLock = new();

    private readonly AudioPlayer _audioPlayer;

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

    public SettingsViewModel(AudioPlayer audioPlayer, DotaTimers timers)
    {
        _audioPlayer = audioPlayer;
        Timers = timers;

        foreach (var dotaTimer in Timers)
        {
            dotaTimer.PropertyChanged -= TimerOnPropertyChanged;
            dotaTimer.PropertyChanged += TimerOnPropertyChanged;
        }
    }

    public bool IsSpeakerMuted => Volume <= 0;

    public bool IsSpeakerOn => Volume > 0;
    
    private ImmutableArray<string> Properties =>
    [
        nameof(DotaTimer.IsEnabled),
        nameof(DotaTimer.IsSoundEnabled),
        nameof(DotaTimer.IsTts),
        nameof(DotaTimer.Reminder)
    ];

    private void TimerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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