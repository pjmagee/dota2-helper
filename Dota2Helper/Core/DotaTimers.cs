using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Dota2Helper.Core;

public class ConfiguredDotaTimers : ObservableCollection<DotaTimer>
{
    public ConfiguredDotaTimers(IConfiguration configuration)
    {
        List<DotaTimer> timers = new List<DotaTimer>();

        TimeSpan ParseTimeSpan(string x) => TimeSpan.ParseExact(x, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
        
        foreach (var item in configuration.GetSection("DotaTimers").GetChildren())
        {
            var isEnabled = bool.Parse(item["IsEnabled"] ?? throw new InvalidOperationException());

            if (isEnabled)
            {
                var label = item["Label"]!;
                var first = ParseTimeSpan(item["First"]!);
                var interval = ParseTimeSpan(item["Interval"]!);
                var reminder = ParseTimeSpan(item["Reminder"]!);
                var audioFile = item["AudioFile"]!;
                var isManualReset = bool.Parse(item["IsManualReset"]! ?? throw new InvalidOperationException());
            
                timers.Add(new DotaTimer(label, first, interval, reminder, audioFile, isManualReset));    
            }
        }
        
        foreach (var timer in timers.OrderBy(x => x.Interval))
        {
            Add(timer);
        }
    }
}