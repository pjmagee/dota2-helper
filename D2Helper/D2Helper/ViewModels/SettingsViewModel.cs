using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Models;
using D2Helper.Services;
using D2Helper.Views;

namespace D2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    readonly TimerService _timerService;
    DotaTimer _selectedTimer;
    double _timerVolume;

    public ObservableCollection<DotaTimer> Timers => _timerService.Timers;

    public DotaTimer SelectedTimer
    {
        get => _selectedTimer;
        set => SetProperty(ref _selectedTimer, value);
    }

    public RelayCommand<DotaTimer> DeleteCommand { get; }
    public RelayCommand<object> BrowseAudioFileCommand { get; }

    public double TimerVolume
    {
        get => _timerVolume;
        set => SetProperty(ref _timerVolume, value);
    }

    public IRelayCommand InstallGSICommand { get; }

    public SettingsViewModel(TimerService timerService)
    {
        _timerService = timerService;
        DeleteCommand = new RelayCommand<DotaTimer>(Delete);
        BrowseAudioFileCommand = new RelayCommand<object>(BrowseAudioFile);
        InstallGSICommand = new RelayCommand(InstallGSI);
        SelectedTimer = _timerService.Timers[0];
    }

    void InstallGSI()
    {
        // TODO: install gsi config into dota2 installation gsi folder
    }

    async void BrowseAudioFile(object obj)
    {
        if (obj is SettingsView view)
        {
            var topLevel = TopLevel.GetTopLevel(view);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Select audio file",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Audio files")
                        {
                            Patterns = new[]
                            {
                                "*.mp3",
                                "*.wav"
                            },
                            MimeTypes = new[]
                            {
                                "audio/*"
                            }
                        }
                    },
                }
            );

            if (files.Count == 1)
            {
                SelectedTimer.AudioFile = files[0].Path.ToString();
            }
        }
    }

    public SettingsViewModel() : this(new TimerService())
    {

    }

    public void Reset()
    {
        _timerService.Reset();
    }

    public void Add()
    {
        _timerService.Timers.Add(new DotaTimer()
            {
                Name = $"Timer {_timerService.Timers.Count + 1}"
            }
        );
    }

    public void Delete(DotaTimer? timer)
    {
        if (timer is null)
        {
            return;
        }

        _timerService.Timers.Remove(timer);
    }
}