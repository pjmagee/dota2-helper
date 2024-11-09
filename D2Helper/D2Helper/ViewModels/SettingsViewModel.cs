﻿using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Features.Audio;
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
    readonly TimerService _timerService;
    readonly AudioService _audioService;
    readonly SettingsService _settingsService;

    DotaTimerViewModel _selectedTimerViewModel;
    TimerStrategy _selectedTimerMode;
    double _volume;
    string _status;
    string _installPath;
    bool _demoMuted;
    bool _isListening;

    public ObservableCollection<DotaTimerViewModel> Timers => _timerService.Timers;
    public RelayCommand<DotaTimerViewModel> DeleteCommand { get; }

    public RelayCommand<DotaTimerViewModel> MoveUpCommand { get; }

    public RelayCommand<DotaTimerViewModel> MoveDownCommand { get; }



    public RelayCommand<SettingsView> SelectFileCommand { get; }

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
                TimerModeCommand.Execute(value);
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
        set
        {
            if (SetProperty(ref _demoMuted, value))
            {
                _settingsService.Settings.DemoMuted = value;
                _settingsService.UpdateSettings(_settingsService.Settings);
            }
        }
    }

    public IRelayCommand PlayAudioFileCommand { get; }

    public bool IsListening
    {
        get => _isListening;
        set => SetProperty(ref _isListening, value);
    }


    public SettingsViewModel(
        TimerService timerService,
        AudioService audioService,
        SettingsService settingsService)
    {
        _timerService = timerService;
        _audioService = audioService;
        _settingsService = settingsService;

        DeleteCommand = new RelayCommand<DotaTimerViewModel>(DeleteTimer);
        SelectFileCommand = new RelayCommand<SettingsView>(SelectFile);
        InstallCommand = new RelayCommand(Install);
        UninstallCommand = new RelayCommand(Uninstall);
        TimerModeCommand = new RelayCommand<TimerStrategy>(SetStrategy);
        PlayAudioFileCommand = new RelayCommand(PlayAudioFile);
        TimerModes =
        [
            new()
            {
                Name = "Real",
                Mode = TimeMode.Real
            },
            new()
            {
                Name = "Demo",
                Mode = TimeMode.Demo
            },
            new()
            {
                Name = "Auto",
                Mode = TimeMode.Auto
            },
        ];

        _selectedTimerMode = TimerModes.FirstOrDefault(tm => _settingsService.Settings.Mode == tm.Mode) ?? TimerModes[^1];
        _selectedTimerViewModel = _timerService.Timers[0];
        _volume = _settingsService.Settings.Volume;
        _demoMuted = _settingsService.Settings.DemoMuted;

        MoveUpCommand = new RelayCommand<DotaTimerViewModel>(MoveUp);
        MoveDownCommand = new RelayCommand<DotaTimerViewModel>(MoveDown);
    }

    void MoveUp(DotaTimerViewModel? obj)
    {
        if (obj is not null)
        {
            var index = _timerService.Timers.IndexOf(obj);
            if (index > 0)
            {
                Timers[index].SortOrder = index - 1;
                Timers.Move(index, index - 1);
                OnPropertyChanged(nameof(Timers));
            }
        }
    }

    void MoveDown(DotaTimerViewModel? obj)
    {
        if (obj is not null)
        {
            var index = Timers.IndexOf(obj);
            if (index < Timers.Count - 1)
            {
                Timers[index].SortOrder = index + 1;
                Timers.Move(index, index + 1);
                OnPropertyChanged(nameof(Timers));
            }
        }
    }

    void PlayAudioFile()
    {
        if (string.IsNullOrWhiteSpace(SelectedTimerViewModel.AudioFile) is false)
        {
            _audioService.Play(SelectedTimerViewModel.AudioFile);
        }
    }

    void SetStrategy(TimerStrategy? option)
    {
        if (option is { Mode: var strategy })
        {
            _settingsService.Settings.Mode = strategy;
            _settingsService.UpdateSettings(_settingsService.Settings);
        }
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