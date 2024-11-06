using System;
using System.Collections.Generic;
using D2Helper.Models;
using D2Helper.Services;

namespace D2Helper.Design;

public class DesignSettingsService : SettingsService
{
    public DesignSettingsService() : base()
    {
        Settings = new Settings
        {
            // All Dota2 Timers
            Timers = new List<DotaTimer>
            {
                new()
                {
                    IsEnabled = true,
                    IsMuted = false,
                    Name = "Starts at 30s",
                    Every = TimeSpan.FromMinutes(1),
                    StartsAfter = TimeSpan.FromSeconds(30),
                    RemindAt = TimeSpan.FromSeconds(15),
                    ExpireAfter = TimeSpan.FromMinutes(20),
                    IsManualReset = false,
                    IsInterval = true,
                },
                new()
                {
                    IsEnabled = true,
                    IsMuted = false,
                    Name = "Expires at 30s",
                    Every = TimeSpan.FromMinutes(1),
                    ExpireAfter = TimeSpan.FromSeconds(30),
                    IsManualReset = false,
                    IsInterval = true,
                },
                new()
                {
                    IsEnabled = true,
                    IsMuted = false,
                    Name = "Manual Reset",
                    Every = TimeSpan.FromSeconds(20),
                    RemindAt = TimeSpan.FromSeconds(15),
                    ExpireAfter = TimeSpan.FromMinutes(20),
                    IsManualReset = true,
                    IsInterval = false
                },
            },
            Volume = 50,
            Mode = GameStateStrategy.Auto,
            DemoMuted = true
        };
    }
}