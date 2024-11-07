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
                    Time = TimeSpan.FromMinutes(1),
                    ShowAfter = TimeSpan.FromSeconds(30),
                    RemindAt = TimeSpan.FromSeconds(15),
                    HideAfter = TimeSpan.FromMinutes(20),
                    IsManualReset = false,
                    IsInterval = true,
                },
                new()
                {
                    IsEnabled = true,
                    IsMuted = false,
                    Name = "Expires at 30s",
                    Time = TimeSpan.FromMinutes(1),
                    HideAfter = TimeSpan.FromSeconds(30),
                    IsManualReset = false,
                    IsInterval = true,
                },
                new()
                {
                    IsEnabled = true,
                    IsMuted = false,
                    Name = "Manual Reset",
                    Time = TimeSpan.FromSeconds(20),
                    RemindAt = TimeSpan.FromSeconds(15),
                    HideAfter = TimeSpan.FromMinutes(20),
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