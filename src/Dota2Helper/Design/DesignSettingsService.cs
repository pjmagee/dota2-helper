using System;
using System.Collections.Generic;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Design;

public class DesignSettingsService : SettingsService
{
    public DesignSettingsService() : base()
    {
        Settings = new Settings();
        ConfigureDesignSettings(Settings);

        DefaultTimers = new List<DotaTimer>();
        ConfigureDesignDefaultTimers(DefaultTimers);
    }

    void ConfigureDesignDefaultTimers(List<DotaTimer> o)
    {
        o.Add(new DotaTimer
            {
                Name = "Roshan",
                Interval = TimeSpan.FromMinutes(8),
                IsInterval = false,
                IsManualReset = true,
                IsEnabled = true,
                IsMuted = false,
                RemindBefore = TimeSpan.FromMinutes(1),
                Visibility = new Visibility()
                {
                    HideAfter = TimeSpan.FromMinutes(1),
                    ShowAfter = TimeSpan.FromMinutes(1)
                },
                Offset = TimeSpan.FromMinutes(0),
                AudioFile = null,
            }
        );
    }

    void ConfigureDesignSettings(Settings o)
    {
        o.Mode = TimeMode.Auto;
        o.Volume = 50;
        o.Theme = "Light";
        o.DemoMuted = true;
        o.SelectedProfileIdx = 0;
        o.Profiles =
        [
            new Profile
            {
                Name = "Default",
                Timers = new List<DotaTimer>
                {
                    new DotaTimer
                    {
                        Name = "Roshan",
                        Interval = TimeSpan.FromMinutes(8),
                        IsInterval = false,
                        IsManualReset = true,
                        IsEnabled = true,
                        IsMuted = false,
                        RemindBefore = TimeSpan.FromMinutes(1),
                        // StopAfter = TimeSpan.FromMinutes(1),
                        // StartAfter = TimeSpan.FromMinutes(1),
                        Offset = TimeSpan.FromMinutes(0),
                        Visibility = new Visibility()
                        {
                            HideAfter = TimeSpan.FromMinutes(1),
                            ShowAfter = TimeSpan.FromMinutes(1)
                        },
                        AudioFile = null,
                    }
                }
            },
        ];
    }
}