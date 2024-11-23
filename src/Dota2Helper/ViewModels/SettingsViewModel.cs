using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Gsi;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;
using Dota2Helper.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dota2Helper.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    readonly ProfileService _profileService;
    readonly IAudioService _audioService;
    readonly ViewModelFactory _viewModelFactory;
    readonly SettingsService _settingsService;
    readonly GsiConfigService _gsiConfigService;

    double _volume;
    bool _demoMuted;
    int _selectedProfileIndex;

    DotaTimerViewModel? _selectedTimerViewModel;

    // ProfileViewModel _selectedProfileViewModel;
    TimerStrategy? _selectedTimerMode;
    ThemeVariant _themeVariant = ThemeVariant.Default;

    public ObservableCollection<ProfileViewModel> Profiles => _profileService.Profiles;
    public ObservableCollection<TimerStrategy> TimerModes { get; set; }
    public IRelayCommand<DotaTimerViewModel> DeleteTimerCommand { get; }
    public IRelayCommand<ProfileViewModel> DeleteProfileCommand { get; }
    public IRelayCommand<SettingsView> SelectFileCommand { get; }
    public IRelayCommand InstallCommand { get; }
    public IRelayCommand UninstallCommand { get; }
    public IRelayCommand OpenFolderCommand { get; set; }
    public IRelayCommand SetModeCommand { get; }
    public IRelayCommand PlayAudioCommand { get; }

    public ThemeVariant ThemeVariant
    {
        get => _themeVariant;
        set
        {
            if (SetProperty(ref _themeVariant, value))
            {
                SetTheme(value);
            }
        }
    }

    void SetTheme(ThemeVariant variant)
    {
        Application.Current!.RequestedThemeVariant = variant;
        _settingsService.Settings.Theme = variant.Key.ToString()!;
        _settingsService.SaveSettings();
    }

    ObservableCollection<DotaTimerViewModel> _timers;

    public ObservableCollection<DotaTimerViewModel> Timers
    {
        get => _timers;
        set => SetProperty(ref _timers, value);
    }

    private ProfileViewModel? _selectedProfileViewModel;

    public ProfileViewModel? SelectedProfileViewModel
    {
        get => _selectedProfileViewModel;
        set
        {
            if (value == null) return;
            SetProperty(ref _selectedProfileViewModel, value);
            _profileService.SelectedProfileViewModel = value;
            Timers = _profileService.SelectedProfileViewModel.Timers;
        }
    }

    public DotaTimerViewModel? SelectedTimerViewModel
    {
        get => _selectedTimerViewModel;
        set => SetProperty(ref _selectedTimerViewModel, value);
    }

    public TimerStrategy SelectedTimerMode
    {
        get => _selectedTimerMode ?? TimerModes[^1];
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

    public int SelectedProfileIndex
    {
        get => _selectedProfileIndex;
        set
        {
            if (SetProperty(ref _selectedProfileIndex, value))
            {
                _settingsService.Settings.SelectedProfileIdx = value;
                _settingsService.UpdateSettings(_settingsService.Settings);
            }
        }
    }

    public ObservableCollection<ThemeVariant> ThemeVariants { get; } =
    [
        ThemeVariant.Default,
        ThemeVariant.Dark,
        ThemeVariant.Light,
    ];

    public string Version => $"Version: {GitVersionInformation.MajorMinorPatch}";

    public SettingsViewModel(
        ProfileService profileService,
        [FromKeyedServices(nameof(AudioService))]
        IAudioService audioService,
        ViewModelFactory viewModelFactory,
        SettingsService settingsService,
        GsiConfigService gsiConfigService)
    {
        _profileService = profileService;
        _audioService = audioService;
        _viewModelFactory = viewModelFactory;

        _settingsService = settingsService;
        _gsiConfigService = gsiConfigService;

        DeleteTimerCommand = new RelayCommand<DotaTimerViewModel>(DeleteTimer);
        DeleteProfileCommand = new RelayCommand<ProfileViewModel>(DeleteProfile);
        SelectFileCommand = new RelayCommand<SettingsView>(SelectFile);
        InstallCommand = new RelayCommand(Install);
        UninstallCommand = new RelayCommand(Uninstall);
        OpenFolderCommand = new RelayCommand(OpenFolder);
        SetModeCommand = new RelayCommand<TimerStrategy>(SetMode);
        PlayAudioCommand = new RelayCommand(PlayAudio);
        TimerModes = new ObservableCollection<TimerStrategy>(TimerStrategy.Modes);

        SelectedProfileViewModel = _profileService.Profiles[_settingsService.Settings.SelectedProfileIdx];
        LoadSavedTheme(_settingsService.Settings.Theme);
        _selectedTimerMode = TimerModes.FirstOrDefault(tm => _settingsService.Settings.Mode == tm.Mode) ?? TimerModes[^1];
        _volume = _settingsService.Settings.Volume;
        _demoMuted = _settingsService.Settings.DemoMuted;
    }

    void LoadSavedTheme(string settingsTheme)
    {
        ThemeVariant = settingsTheme switch
        {
            "Dark" => ThemeVariant.Dark,
            "Light" => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };
    }

    void OpenFolder()
    {
        _gsiConfigService.OpenGsiFolder();
    }

    void PlayAudio()
    {
        if (string.IsNullOrWhiteSpace(SelectedTimerViewModel?.AudioFile) is false)
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
        try
        {
            if (view is not null)
            {
                var topLevel = TopLevel.GetTopLevel(view)!;
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new AudioFilePickerOptions());

                if (files.Count == 1)
                {
                    SelectedTimerViewModel!.AudioFile = files[0].Path.ToString();
                }
            }
        }
        catch (Exception)
        {

        }
    }

    public void DefaultTimers()
    {
        _profileService.DefaultTimers();
    }

    public void AddProfile()
    {
        _profileService.Profiles.Add(_viewModelFactory.Create(new Profile
                {
                    Name = $"Profile {_profileService.Profiles.Count + 1}"
                }
            )
        );
    }

    public void AddTimer()
    {
        var dotaTimer = new DotaTimer
        {
            Name = $"Timer {SelectedProfileViewModel.Timers.Count + 1}",
            Time = TimeSpan.FromMinutes(1),
            IsEnabled = false,
            IsMuted = false,
            IsManualReset = false,
            IsInterval = true
        };

        SelectedProfileViewModel.Timers.Add(_viewModelFactory.Create(dotaTimer));
    }

    public void DeleteTimer(DotaTimerViewModel? timer)
    {
        if (timer is not null)
        {
            foreach (var profile in _profileService.Profiles)
            {
                if (profile.Timers.Contains(timer))
                {
                    profile.Timers.Remove(timer);
                }
            }
        }
    }

    public void DeleteProfile(ProfileViewModel? profile)
    {
        if (profile is not null)
        {
            if (_profileService.Profiles.Count == 1)
            {
                return;
            }

            _profileService.Profiles.Remove(profile);
        }
    }
}