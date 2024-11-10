using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.Audio;
using D2Helper.Features.Gsi;
using D2Helper.Features.Settings;
using D2Helper.Features.TimeProvider;
using D2Helper.Features.Timers;
using D2Helper.Views;

namespace D2Helper.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;

public class SettingsViewModel : ViewModelBase
{
    double _volume;
    string _status;
    string _installPath;
    bool _demoMuted;
    bool _isListening;

    readonly TimerService _timerService;
    readonly AudioService _audioService;
    readonly SettingsService _settingsService;
    readonly GsiConfigService _gsiConfigService;

    DotaTimerViewModel _selectedTimerViewModel;
    TimerStrategy _selectedTimerMode;

    public ObservableCollection<DotaTimerViewModel> Timers => _timerService.Timers;
    public ObservableCollection<TimerStrategy> TimerModes { get; set; }
    public IRelayCommand<DotaTimerViewModel> DeleteCommand { get; }
    public IRelayCommand<DotaTimerViewModel> MoveUpCommand { get; }
    public IRelayCommand<DotaTimerViewModel> MoveDownCommand { get; }
    public IRelayCommand<SettingsView> SelectFileCommand { get; }
    public IRelayCommand InstallCommand { get; }
    public IRelayCommand UninstallCommand { get; }
    public IRelayCommand OpenFolderCommand { get; set; }
    public IRelayCommand SetModeCommand { get; }
    public IRelayCommand PlayAudioCommand { get; }

    public DotaTimerViewModel SelectedTimerViewModel
    {
        get => _selectedTimerViewModel;
        set => SetProperty(ref _selectedTimerViewModel, value);
    }

    public TimerStrategy SelectedTimerMode
    {
        get => _selectedTimerMode;
        set
        {
            if (SetProperty(ref _selectedTimerMode, value))
            {
                SetModeCommand.Execute(value);
                _settingsService.Settings.Mode = value.Mode;
                _settingsService.UpdateSettings(_settingsService.Settings);
            }
        }
    }

    public double Volume
    {
        get => _volume;
        set
        {
            if (SetProperty(ref _volume, value))
            {
                _settingsService.Settings.Volume = value;
                _settingsService.UpdateSettings(_settingsService.Settings);
            }
        }
    }

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
        set
        {
            if (SetProperty(ref _demoMuted, value))
            {
                _settingsService.Settings.DemoMuted = value;
                _settingsService.UpdateSettings(_settingsService.Settings);
            }
        }
    }

    public bool IsListening
    {
        get => _isListening;
        set => SetProperty(ref _isListening, value);
    }

    public SettingsViewModel(
        TimerService timerService,
        AudioService audioService,
        SettingsService settingsService,
        GsiConfigService gsiConfigService)
    {
        _timerService = timerService;
        _audioService = audioService;
        _settingsService = settingsService;
        _gsiConfigService = gsiConfigService;

        DeleteCommand = new RelayCommand<DotaTimerViewModel>(DeleteTimer);
        SelectFileCommand = new RelayCommand<SettingsView>(SelectFile);
        InstallCommand = new RelayCommand(Install);
        UninstallCommand = new RelayCommand(Uninstall);
        OpenFolderCommand = new RelayCommand(OpenFolder);
        SetModeCommand = new RelayCommand<TimerStrategy>(SetMode);
        PlayAudioCommand = new RelayCommand(PlayAudio);
        MoveUpCommand = new RelayCommand<DotaTimerViewModel>(MoveUp);
        MoveDownCommand = new RelayCommand<DotaTimerViewModel>(MoveDown);

        TimerModes =
        [
            new() { Name = "Real", Mode = TimeMode.Real },
            new() { Name = "Demo", Mode = TimeMode.Demo },
            new() { Name = "Auto", Mode = TimeMode.Auto },
        ];

        _selectedTimerMode = TimerModes.FirstOrDefault(tm => _settingsService.Settings.Mode == tm.Mode) ?? TimerModes[^1];
        _selectedTimerViewModel = _timerService.Timers[0];
        _volume = _settingsService.Settings.Volume;
        _demoMuted = _settingsService.Settings.DemoMuted;
    }

    void OpenFolder()
    {
        _gsiConfigService.OpenGsiFolder();
    }


    void MoveUp(DotaTimerViewModel? timerVm)
    {
        if (timerVm is not null)
        {
            var index = _timerService.Timers.IndexOf(timerVm);
            if (index > 0)
            {
                Timers[index].SortOrder = index - 1;
                Timers.Move(index, index - 1);
                OnPropertyChanged(nameof(Timers));
            }
        }
    }

    void MoveDown(DotaTimerViewModel? timerVm)
    {
        if (timerVm is not null)
        {
            var index = Timers.IndexOf(timerVm);
            if (index < Timers.Count - 1)
            {
                Timers[index].SortOrder = index + 1;
                Timers.Move(index, index + 1);
                OnPropertyChanged(nameof(Timers));
            }
        }
    }

    void PlayAudio()
    {
        if (string.IsNullOrWhiteSpace(SelectedTimerViewModel.AudioFile) is false)
        {
            _audioService.Play(SelectedTimerViewModel.AudioFile);
        }
    }

    void SetMode(TimerStrategy? option)
    {
        if (option is { Mode: var strategy })
        {
            _settingsService.Settings.Mode = strategy;
            _settingsService.UpdateSettings(_settingsService.Settings);
        }
    }

    void Install() => _gsiConfigService.Install();

    void Uninstall() => _gsiConfigService.Uninstall();

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
                SelectedTimerViewModel.AudioFile = files[0].Path.ToString();
            }
        }
    }

    public void Reset()
    {
        _timerService.Default();
    }

    public void Add()
    {
        _timerService.Timers.Add(new DotaTimerViewModel(new DotaTimer()
            {
                Name = $"Timer {_timerService.Timers.Count + 1}",
                Time = TimeSpan.FromMinutes(1),
                IsEnabled = false,
                IsMuted = false,
                IsManualReset = false,
                IsInterval = true
            }
        ));
    }

    public void DeleteTimer(DotaTimerViewModel? timer)
    {
        if (timer is not null)
        {
            _timerService.Timers.Remove(timer);
        }
    }
}