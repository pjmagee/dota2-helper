using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using D2Helper.Models;
using D2Helper.ViewModels;
using FluentAvalonia.Core;
using LibVLCSharp.Shared;

namespace D2Helper.Services;

public class TimerAudioQueueService : BackgroundWorker
{
    LibVLC LibVlc { get; } = new();

    public Queue<string> AudioQueue { get; } = new();

    public void Add(string audioFile)
    {
        // Ensure no duplicate added
        if (!AudioQueue.Contains(audioFile))
        {
            AudioQueue.Enqueue(audioFile);
        }
    }

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        while (!CancellationPending)
        {
            if (AudioQueue.Count > 0)
            {
                var audioFile = AudioQueue.Dequeue();

                using var media = new Media(LibVlc, audioFile, FromType.FromLocation);
                {
                    using var player = new MediaPlayer(media);
                    {
                        player.Play();
                    }
                }
            }
        }
    }
}

public class TimerService
{
    readonly SettingsService _settingsService;
    public ObservableCollection<DotaTimerViewModel> Timers { get; } = new();

    public TimerService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        Timers.CollectionChanged -= TimersChanged;
        Timers.CollectionChanged += TimersChanged;

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

    readonly HashSet<string> _ignoredProperties = new()
    {
        nameof(DotaTimerViewModel.TimeRemaining),
        nameof(DotaTimerViewModel.IsAlerting),
        nameof(DotaTimerViewModel.IsExpired),
        nameof(DotaTimerViewModel.IsVisible),
        nameof(DotaTimerViewModel.IsResetRequired)
    };

    void TimerChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(_ignoredProperties.Contains(e.PropertyName)) return;

        _settingsService.Settings.Timers.Clear();
        _settingsService.Settings.Timers.AddRange(Timers.Select(x => new DotaTimer()
        {
            Name = x.Name,

            IsMuted = x.IsMuted,
            IsManualReset = x.IsManualReset,
            IsInterval = x.IsInterval,
            IsEnabled = x.IsEnabled,

            Speech = x.Speech,
            AudioFile = x.AudioFile,

            Time = x.Time,
            RemindAt = x.RemindAt,
            HideAfter = x.HideAfter,
            ShowAfter = x.ShowAfter
        }));

        _settingsService.SaveSettings();
    }

    public void Default()
    {
        Timers.Clear();

        // Roshan Spawn
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Roshan",
            Time = TimeSpan.FromMinutes(11),
            RemindAt = TimeSpan.FromMinutes(1),
            HideAfter = TimeSpan.FromMinutes(60),
            IsManualReset = true,
            IsMuted = false,
            IsInterval = true,
            IsEnabled = false,
        }));

        // Tormentor Radiant
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Tormentor Radiant",
            Time = TimeSpan.FromMinutes(10),
            RemindAt = TimeSpan.FromMinutes(1),
            HideAfter = TimeSpan.FromMinutes(35),
            IsMuted = false,
            IsManualReset = true,
            IsInterval = true,
            IsEnabled = true
        }));

        // Tormentor Dire
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Tormentor Dire",
            Time = TimeSpan.FromMinutes(10),
            RemindAt = TimeSpan.FromMinutes(1),
            HideAfter = TimeSpan.FromMinutes(35),
            IsEnabled = true,
            IsMuted = false,
            IsManualReset = true,
            IsInterval = true
        }));

        // Powerup Rune
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Power Rune",
            Time = TimeSpan.FromMinutes(2),
            RemindAt = TimeSpan.FromMinutes(0.25),
            HideAfter = TimeSpan.FromMinutes(20),
            IsMuted = false,
            IsManualReset = false,
            IsInterval = true,
            IsEnabled = true,
        }));

        // Bounty Rune
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Bounty Rune",
            Time = TimeSpan.FromMinutes(3),
            RemindAt = TimeSpan.FromSeconds(20),
            HideAfter = TimeSpan.FromMinutes(30),
            IsMuted = false,
            IsManualReset = false,
            IsInterval = true,
            IsEnabled = true,
        }));

        // Wisdom Rune
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Wisdom Rune",
            Time = TimeSpan.FromMinutes(7),
            RemindAt = TimeSpan.FromSeconds(45),
            HideAfter = TimeSpan.FromMinutes(30),
            IsMuted = false,
            IsInterval = true,
            IsManualReset = false,
            IsEnabled = true,
        }));

        // Stack Camp
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Stack Camp",
            Time = TimeSpan.FromMinutes(1),
            RemindAt = TimeSpan.FromSeconds(15),
            HideAfter = TimeSpan.FromMinutes(30),
            ShowAfter = TimeSpan.FromMinutes(2),
            IsMuted = false,
            IsManualReset = false,
            IsInterval = true,
            IsEnabled = true,
        }));

        // Pull (15s)
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Pull (15s)",
            Time = TimeSpan.FromSeconds(15),
            RemindAt = TimeSpan.FromSeconds(5),
            HideAfter = TimeSpan.FromMinutes(20),
            IsEnabled = true,
            IsInterval = false,
            IsMuted = false,
            IsManualReset = false,
        }));

        // Pull (45s)
        Timers.Add(new DotaTimerViewModel(new DotaTimer()
        {
            Name = "Pull (45s)",
            Time = TimeSpan.FromSeconds(45),
            RemindAt = TimeSpan.FromSeconds(5),
            HideAfter = TimeSpan.FromMinutes(20),
            IsEnabled = true,
            IsMuted = false,
            IsInterval = false,
            IsManualReset = false
        }));
    }
}