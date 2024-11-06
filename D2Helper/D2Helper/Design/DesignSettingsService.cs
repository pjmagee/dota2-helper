using System;
using System.Collections.Generic;
using D2Helper.Models;
using D2Helper.Services;

namespace D2Helper.Design;

public class DesignSettingsService : SettingsService
{
    public DesignSettingsService()
    {
        Settings = new Settings
        {
            // All Dota2 Timers
            Timers = new List<DotaTimer>
            {
                new DotaTimer { Name = "Roshan", Every = TimeSpan.FromMinutes(8), RemindAt = TimeSpan.FromMinutes(5) },
                new DotaTimer { Name = "Power Rune", Every = TimeSpan.FromMinutes(2), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Bounty Rune", Every = TimeSpan.FromMinutes(5), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Neutral Items", Every = TimeSpan.FromMinutes(7), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Outpost", Every = TimeSpan.FromMinutes(10), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Glyph", Every = TimeSpan.FromMinutes(5), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Scan", Every = TimeSpan.FromMinutes(4), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Aegis", Every = TimeSpan.FromMinutes(5), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Cheese", Every = TimeSpan.FromMinutes(5), RemindAt = TimeSpan.FromMinutes(1) },
                new DotaTimer { Name = "Aghanim's Blessing", Every = TimeSpan.FromMinutes(5), RemindAt = TimeSpan.FromMinutes(1) }
            },
            Volume = 50,
            Mode = GameStateStrategy.Auto,
            DemoMuted = true
        };
    }
}