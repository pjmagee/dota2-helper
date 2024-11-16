using System;
using System.Collections.Generic;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Design;

public class DesignSettingsService : SettingsService
{
    public DesignSettingsService() : base()
    {
        Settings = new Settings
        {
            SelectedProfileIdx = 0,
            Profiles = new List<Profile>
            {
                new()
                {
                    Name = "Profile 1",
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
                    }
                }
            },
            Volume = 50,
            Mode = TimeMode.Auto,
            DemoMuted = true
        };
    }
}