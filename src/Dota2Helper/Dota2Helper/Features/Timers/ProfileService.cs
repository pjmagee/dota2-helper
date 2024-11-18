using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Features.Timers;

public class ProfileService
{
    readonly SettingsService _settingsService;
    readonly ViewModelFactory _factory;

    ProfileViewModel _selectedProfileViewModel;

    public ProfileViewModel SelectedProfileViewModel
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
                Profiles.Add(factory.Create(profile));
            }
        }
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
                item.PropertyChanged += SaveConfiguration;
                item.Timers.CollectionChanged += SaveConfiguration;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (ProfileViewModel item in e.OldItems ?? Array.Empty<ProfileViewModel>())
            {
                item.PropertyChanged -= SaveConfiguration;
                item.Timers.CollectionChanged -= SaveConfiguration;
            }
        }
    }

    void SaveConfiguration(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is DotaTimerViewModel timer && !string.IsNullOrWhiteSpace(e.PropertyName))
        {
            if (_ignoredProperties.Contains(e.PropertyName)) return;
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
                        Time = y.Time,
                        RemindAt = y.RemindAt,
                        HideAfter = y.HideAfter,
                        ShowAfter = y.ShowAfter
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
        nameof(DotaTimerViewModel.IsAlerting),
        nameof(DotaTimerViewModel.IsExpired),
        nameof(DotaTimerViewModel.IsVisible),
        nameof(DotaTimerViewModel.IsSoundPlayed),
        nameof(DotaTimerViewModel.IsManualTimerReset),
        nameof(DotaTimerViewModel.IsResetRequired)
    ];




    public void DefaultTimers()
    {
        SelectedProfileViewModel.Timers.Clear();

        foreach (var dotaTimer in _settingsService.DefaultTimers)
        {
            SelectedProfileViewModel.Timers.Add(_factory.Create(dotaTimer));
        }
    }
}