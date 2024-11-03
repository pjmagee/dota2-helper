using System;
using System.Collections.ObjectModel;
using D2Helper.Models;

namespace D2Helper.Services;

public class TimerService
{
    public ObservableCollection<DotaTimer> Timers { get; } = new();

    public TimerService()
    {
        Reset();
    }

    public void Reset()
    {
        Timers.Clear();

        // Roshan Spawn
        Timers.Add(new DotaTimer()
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
        Timers.Add(new DotaTimer()
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
        Timers.Add(new DotaTimer()
        {
            Name = "Tormentor Dire",
            Every = TimeSpan.FromMinutes(10),
            RemindAt = TimeSpan.FromMinutes(1),
            DisableAfter = TimeSpan.FromMinutes(35),
            IsManualReset = true,
            IsInterval = true
        });

        // Powerup Rune
        Timers.Add(new DotaTimer()
        {
            Name = "Power Rune",
            Every = TimeSpan.FromMinutes(2),
            RemindAt = TimeSpan.FromMinutes(0.25),
            DisableAfter = TimeSpan.FromMinutes(20),
            IsInterval = true,
            IsEnabled = true,
        });

        // Bounty Rune
        Timers.Add(new DotaTimer()
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
        Timers.Add(new DotaTimer()
        {
            Name = "Wisdom Rune",
            Every = TimeSpan.FromMinutes(7),
            RemindAt = TimeSpan.FromSeconds(45),
            DisableAfter = TimeSpan.FromMinutes(30),
            IsInterval = true,
            IsEnabled = true,
        });

        // Stack Camp
        Timers.Add(new DotaTimer()
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
        Timers.Add(new DotaTimer()
        {
            Name = "Pull (15s)",
            Every = TimeSpan.FromSeconds(15),
            RemindAt = TimeSpan.FromSeconds(5),
            DisableAfter = TimeSpan.FromMinutes(20),
            IsInterval = false,
            IsManualReset = false
        });

        // Pull (45s)
        Timers.Add(new DotaTimer()
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