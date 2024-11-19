using System;
using System.Collections.Generic;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Design;

public class DesignSettingsService : SettingsService
{
    public DesignSettingsService() : base()
    {
        Settings = new Settings()
        {
            DemoMuted = true,
            Volume = 90,
            SelectedProfileIdx = 0,
            Mode = TimeMode.Auto,
            Profiles = new List<Profile>()
            {
                new Profile()
                {
                    Name = "Default",
                    Timers = new List<DotaTimer>()
                    {

                    }
                }
            }
        };

        DefaultTimers = new List<DotaTimer>()
        {
            new DotaTimer()
            {
                Name = "Roshan",
                Time = TimeSpan.FromMinutes(1),
                IsEnabled = true,
                IsInterval = true,
                IsMuted = false,
                IsManualReset = true,
                RemindAt = null,
                StartAfter = null,
                StopAfter = null
            }
        };
    }
}