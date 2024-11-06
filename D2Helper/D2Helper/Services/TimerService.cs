using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using D2Helper.Models;
using D2Helper.ViewModels;

namespace D2Helper.Services;

public class TimerService
{
    readonly SettingsService _settingsService;
    public ObservableCollection<DotaTimerViewModel> Timers { get; } = new();

    public TimerService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        if (_settingsService.Settings.Timers.Count == 0)
        {
            Default();
        }
        else
        {
            foreach (var timer in _settingsService.Settings.Timers)
            {
                Timers.Add(new DotaTimerViewModel(timer));
            }
        }

        Timers.CollectionChanged += TimersChanged;
    }

    void TimersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (DotaTimerViewModel item in e.NewItems)
            {
                item.PropertyChanged += TimerChanged;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (DotaTimerViewModel item in e.OldItems)
            {
                item.PropertyChanged -= TimerChanged;
            }
        }
    }

    void TimerChanged(object? sender, PropertyChangedEventArgs e)
    {
        _settingsService.Settings.Timers.Clear();
        _settingsService.Settings.Timers.AddRange(Timers.Select(x => new DotaTimer()
        {
            Name = x.Name,
            Every = x.Every,
            RemindAt = x.RemindAt,
            DisableAfter = x.DisableAfter,
            IsManualReset = x.IsManualReset,
            IsInterval = x.IsInterval,
            IsEnabled = x.IsEnabled,
            Starts = x.Starts
        }));

        _settingsService.SaveSettings();
    }

    public void Default()
    {
        Timers.Clear();

        // Roshan Spawn
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Roshan",
            Every = TimeSpan.FromMinutes(11),
            RemindAt = TimeSpan.FromMinutes(1),
            DisableAfter = TimeSpan.FromMinutes(60),
            IsManualReset = true,
            IsInterval = true,
            IsEnabled = false,
        });

        // Tormentor Radiant
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Tormentor Radiant",
            Every = TimeSpan.FromMinutes(10),
            RemindAt = TimeSpan.FromMinutes(1),
            DisableAfter = TimeSpan.FromMinutes(35),
            IsManualReset = true,
            IsInterval = true,
            IsEnabled = true
        });

        // Tormentor Dire
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Tormentor Dire",
            Every = TimeSpan.FromMinutes(10),
            RemindAt = TimeSpan.FromMinutes(1),
            DisableAfter = TimeSpan.FromMinutes(35),
            IsManualReset = true,
            IsInterval = true
        });

        // Powerup Rune
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Power Rune",
            Every = TimeSpan.FromMinutes(2),
            RemindAt = TimeSpan.FromMinutes(0.25),
            DisableAfter = TimeSpan.FromMinutes(20),
            IsInterval = true,
            IsEnabled = true,
        });

        // Bounty Rune
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Bounty Rune",
            Every = TimeSpan.FromMinutes(3),
            RemindAt = TimeSpan.FromSeconds(20),
            DisableAfter = TimeSpan.FromMinutes(30),
            IsManualReset = false,
            IsInterval = true,
            IsEnabled = true,
        });

        // Wisdom Rune
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Wisdom Rune",
            Every = TimeSpan.FromMinutes(7),
            RemindAt = TimeSpan.FromSeconds(45),
            DisableAfter = TimeSpan.FromMinutes(30),
            IsInterval = true,
            IsEnabled = true,
        });

        // Stack Camp
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Stack Camp",
            Every = TimeSpan.FromMinutes(1),
            RemindAt = TimeSpan.FromSeconds(15),
            DisableAfter = TimeSpan.FromMinutes(30),
            Starts = TimeSpan.FromMinutes(2),
            IsInterval = true,
            IsEnabled = true,
        });

        // Pull (15s)
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Pull (15s)",
            Every = TimeSpan.FromSeconds(15),
            RemindAt = TimeSpan.FromSeconds(5),
            DisableAfter = TimeSpan.FromMinutes(20),
            IsInterval = false,
            IsManualReset = false
        });

        // Pull (45s)
        Timers.Add(new DotaTimerViewModel()
        {
            Name = "Pull (45s)",
            Every = TimeSpan.FromSeconds(45),
            RemindAt = TimeSpan.FromSeconds(5),
            DisableAfter = TimeSpan.FromMinutes(20),
            IsInterval = false,
            IsManualReset = false
        });
    }

}