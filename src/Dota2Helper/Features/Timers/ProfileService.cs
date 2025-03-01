using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Dota2Helper.Features.Settings;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Features.Timers;

public class ProfileService
{
    readonly SettingsService _settingsService;
    readonly ViewModelFactory _factory;

    ProfileViewModel? _selectedProfileViewModel;

    public ProfileViewModel? SelectedProfileViewModel
    {
        get => _selectedProfileViewModel;
        set => _selectedProfileViewModel = value;
    }

    public ObservableCollection<ProfileViewModel> Profiles { get; } = new();

    public ProfileService(SettingsService settingsService, ViewModelFactory factory)
    {
        _settingsService = settingsService;
        _factory = factory;

        Profiles.CollectionChanged -= ProfilesChanged;
        Profiles.CollectionChanged += ProfilesChanged;

        if (_settingsService.Settings.Profiles.Count == 0)
        {
            DefaultTimers();
        }
        else
        {
            foreach (var profile in _settingsService.Settings.Profiles.ToList())
            {
                var profileViewModel = _factory.Create(profile);
                Profiles.Add(profileViewModel);
            }
        }

        SelectedProfileViewModel = Profiles[_settingsService.Settings.SelectedProfileIdx];
    }

    void SaveConfiguration(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SaveConfiguration();
    }

    void ProfilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SaveConfiguration();

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (ProfileViewModel item in e.NewItems ?? Array.Empty<ProfileViewModel>())
            {
                RegisterProfile(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (ProfileViewModel item in e.OldItems ?? Array.Empty<ProfileViewModel>())
            {
                UnregisterProfile(item);
            }
        }
    }

    void UnregisterProfile(ProfileViewModel item)
    {
        item.PropertyChanged -= SaveConfiguration;
        item.Timers.CollectionChanged -= SaveConfiguration;

        foreach (var timer in item.Timers)
        {
            timer.PropertyChanged -= SaveConfiguration;
        }
    }

    void RegisterProfile(ProfileViewModel item)
    {
        item.PropertyChanged += SaveConfiguration;
        item.Timers.CollectionChanged += SaveConfiguration;

        foreach (var timer in item.Timers)
        {
            timer.PropertyChanged -= SaveConfiguration;
            timer.PropertyChanged += SaveConfiguration;
        }
    }

    void SaveConfiguration(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is DotaTimerViewModel && !string.IsNullOrWhiteSpace(e.PropertyName))
        {
            if (_ignoredProperties.Contains(e.PropertyName)) return;
            SaveConfiguration();
        }
        else if (sender is ProfileViewModel && !string.IsNullOrWhiteSpace(e.PropertyName))
        {
            SaveConfiguration();
        }
    }

    void SaveConfiguration()
    {
        var profiles = Profiles.Select(x => new Profile
            {
                Name = x.Name,
                Timers = x.Timers.Select(y => new DotaTimer
                    {
                        Name = y.Name,
                        IsMuted = y.IsMuted,
                        IsManualReset = y.IsManualReset,
                        IsInterval = y.IsInterval,
                        IsEnabled = y.IsEnabled,
                        AudioFile = y.AudioFile,
                        Interval = y.Interval,
                        RemindBefore = y.RemindAt,
                        Offset = y.Offset,
                        Visibility = new Visibility()
                        {
                            HideAfter = y.HideAfter,
                            ShowAfter = y.ShowAfter
                        }
                    }
                ).ToList()
            }
        ).ToList();

        _settingsService.Settings.Profiles.Clear();
        _settingsService.Settings.Profiles.AddRange(profiles);
        _settingsService.SaveSettings();
    }

    readonly HashSet<string> _ignoredProperties =
    [
        nameof(DotaTimerViewModel.TimeRemaining),
        nameof(DotaTimerViewModel.IsAlertable),
        nameof(DotaTimerViewModel.IsStopped),
        nameof(DotaTimerViewModel.IsStarted),
        nameof(DotaTimerViewModel.IsVisible),
        nameof(DotaTimerViewModel.IsSoundPlayed),
        nameof(DotaTimerViewModel.IsManualTimerReset),
        nameof(DotaTimerViewModel.IsResetRequired)
    ];


    public void DefaultTimers()
    {
        if (SelectedProfileViewModel is not null)
        {
            SelectedProfileViewModel.Timers.Clear();

            foreach (var dotaTimer in _settingsService.DefaultTimers)
            {
                SelectedProfileViewModel.Timers.Add(_factory.Create(dotaTimer));
            }
        }
    }
}