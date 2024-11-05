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
    readonly IStrategyProvider _strategyProvider;

    DotaTimer _selectedTimer;
    double _timerVolume;
    TimerStrategy _selectedTimerMode;
    string _status;
    string _installPath;
    bool _demoMuted;

    public ObservableCollection<DotaTimer> Timers => _timerService.Timers;

    public RelayCommand<DotaTimer> DeleteCommand { get; }
    public RelayCommand<SettingsView> BrowseAudioFileCommand { get; }

    public DotaTimer SelectedTimer
    {
        get => _selectedTimer;
        set => SetProperty(ref _selectedTimer, value);
    }

    public TimerStrategy SelectedTimerMode
    {
        get => _selectedTimerMode;
        set
        {
            SetProperty(ref _selectedTimerMode, value);
            TimerModeCommand.Execute(value);
        }
    }

    public double TimerVolume
    {
        get => _timerVolume;
        set => SetProperty(ref _timerVolume, value);
    }

    public ObservableCollection<TimerStrategy> TimerModes { get; set; }

    public IRelayCommand InstallCommand { get; }
    public IRelayCommand UninstallCommand { get; }
    public IRelayCommand TimerModeCommand { get; }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string InstallPath
    {
        get => _installPath;
        set => SetProperty(ref _installPath, value);
    }

    public bool DemoMuted
    {
        get => _demoMuted;
        set => SetProperty(ref _demoMuted, value);
    }


    public SettingsViewModel(TimerService timerService, IStrategyProvider strategyProvider)
    {
        _timerService = timerService;
        _strategyProvider = strategyProvider;

        DeleteCommand = new RelayCommand<DotaTimer>(DeleteTimer);
        BrowseAudioFileCommand = new RelayCommand<SettingsView>(SelectFile);
        InstallCommand = new RelayCommand(Install);
        UninstallCommand = new RelayCommand(Uninstall);
        SelectedTimer = _timerService.Timers[0];
        TimerModeCommand = new RelayCommand<TimerStrategy>(SetStrategy);
        TimerModes =
        [
            new()
            {
                Name = "Real",
                Strategy = GameStateStrategy.Real
            },
            new()
            {
                Name = "Demo",
                Strategy = GameStateStrategy.Demo
            },
            new()
            {
                Name = "Auto",
                Strategy = GameStateStrategy.Auto
            },
        ];

        SelectedTimerMode = TimerModes[^1];
        TimerVolume = 50;
    }

    void SetStrategy(TimerStrategy? option)
    {
        if (option is { Strategy: var strategy })
        {
            _strategyProvider.Strategy = strategy;
        }
    }

    public SettingsViewModel() : this(new TimerService(), new DynamicProvider(new RealGameTimeProvider(), new DemoGameTimeProvider()))
    {

    }

    void Install()
    {
        // TODO: install gsi config into dota2 installation gsi folder
    }

    void Uninstall()
    {
        // TODO: uninstall gsi config from dota2 installation gsi folder
    }

    async void SelectFile(SettingsView? view)
    {
        if (view is not null)
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

    public void DeleteTimer(DotaTimer? timer)
    {
        if (timer is not null)
        {
            _timerService.Timers.Remove(timer);
        }
    }
}