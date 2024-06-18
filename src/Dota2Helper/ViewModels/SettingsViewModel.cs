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
    private static object _writerLock = new();

    private readonly AudioPlayer _audioPlayer;
    private ObservableCollection<DotaTimer>? _timers;

    public ObservableCollection<DotaTimer>? Timers
    {
        get => _timers;
        set => this.RaiseAndSetIfChanged(ref _timers, value);
    }

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

    private void TimerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is DotaTimer timer)
        {
            lock (_writerLock)
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