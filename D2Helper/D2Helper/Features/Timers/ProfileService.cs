using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using D2Helper.Features.Settings;
using D2Helper.ViewModels;

namespace D2Helper.Features.Timers;

public class ProfileService
{
    readonly SettingsService _settingsService;

    public ObservableCollection<ProfileViewModel> Profiles { get; } = new();

    public ProfileViewModel SelectedProfileViewModel { get; set; }

    public ProfileService(SettingsService settingsService)
    {
        _settingsService = settingsService;

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
                Profiles.Add(new ProfileViewModel(profile));
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
            foreach (ProfileViewModel item in e.NewItems)
            {
                item.PropertyChanged += SaveConfiguration;
                item.Timers.CollectionChanged += SaveConfiguration;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (ProfileViewModel item in e.OldItems)
            {
                item.PropertyChanged -= SaveConfiguration;
                item.Timers.CollectionChanged -= SaveConfiguration;
            }
        }
    }

    void SaveConfiguration(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is DotaTimerViewModel timer)
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

    readonly HashSet<string> _ignoredProperties = new()
    {
        nameof(DotaTimerViewModel.TimeRemaining),
        nameof(DotaTimerViewModel.IsAlerting),
        nameof(DotaTimerViewModel.IsExpired),
        nameof(DotaTimerViewModel.IsVisible),
        nameof(DotaTimerViewModel.IsResetRequired)
    };

    public void DefaultTimers()
    {
        SelectedProfileViewModel.Timers.Clear();
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Roshan",
                    Time = TimeSpan.FromMinutes(11),
                    RemindAt = TimeSpan.FromMinutes(1),
                    HideAfter = TimeSpan.FromMinutes(60),
                    IsManualReset = true,
                    IsMuted = false,
                    IsInterval = true,
                    IsEnabled = false
                }
            )
        );

        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Tormentor Radiant",
                    Time = TimeSpan.FromMinutes(10),
                    RemindAt = TimeSpan.FromMinutes(1),
                    HideAfter = TimeSpan.FromMinutes(35),
                    IsMuted = false,
                    IsManualReset = true,
                    IsInterval = true,
                    IsEnabled = true
                }
            )
        );

        // Tormentor Dire
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Tormentor Dire",
                    Time = TimeSpan.FromMinutes(10),
                    RemindAt = TimeSpan.FromMinutes(1),
                    HideAfter = TimeSpan.FromMinutes(35),
                    IsEnabled = true,
                    IsMuted = false,
                    IsManualReset = true,
                    IsInterval = true
                }
            )
        );

        // Powerup Rune
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Power Rune",
                    Time = TimeSpan.FromMinutes(2),
                    RemindAt = TimeSpan.FromMinutes(0.25),
                    HideAfter = TimeSpan.FromMinutes(20),
                    IsMuted = false,
                    IsManualReset = false,
                    IsInterval = true,
                    IsEnabled = true
                }
            )
        );

        // Bounty Rune
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Bounty Rune",
                    Time = TimeSpan.FromMinutes(3),
                    RemindAt = TimeSpan.FromSeconds(20),
                    HideAfter = TimeSpan.FromMinutes(30),
                    IsMuted = false,
                    IsManualReset = false,
                    IsInterval = true,
                    IsEnabled = true
                }
            )
        );

        // Wisdom Rune
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Wisdom Rune",
                    Time = TimeSpan.FromMinutes(7),
                    RemindAt = TimeSpan.FromSeconds(45),
                    HideAfter = TimeSpan.FromMinutes(30),
                    IsMuted = false,
                    IsInterval = true,
                    IsManualReset = false,
                    IsEnabled = true
                }
            )
        );

        // Stack Camp
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Stack Camp",
                    Time = TimeSpan.FromMinutes(1),
                    RemindAt = TimeSpan.FromSeconds(15),
                    HideAfter = TimeSpan.FromMinutes(30),
                    ShowAfter = TimeSpan.FromMinutes(2),
                    IsMuted = false,
                    IsManualReset = false,
                    IsInterval = true,
                    IsEnabled = true
                }
            )
        );

        // Pull (15s)
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Pull (15s)",
                    Time = TimeSpan.FromSeconds(15),
                    RemindAt = TimeSpan.FromSeconds(5),
                    HideAfter = TimeSpan.FromMinutes(20),
                    IsEnabled = true,
                    IsInterval = false,
                    IsMuted = false,
                    IsManualReset = false
                }
            )
        );

        // Pull (45s)
        SelectedProfileViewModel.Timers.Add(new DotaTimerViewModel(new DotaTimer
                {
                    Name = "Pull (45s)",
                    Time = TimeSpan.FromSeconds(45),
                    RemindAt = TimeSpan.FromSeconds(5),
                    HideAfter = TimeSpan.FromMinutes(20),
                    IsEnabled = true,
                    IsMuted = false,
                    IsInterval = false,
                    IsManualReset = false
                }
            )
        );
    }
}