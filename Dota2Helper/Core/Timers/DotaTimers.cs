using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Dota2Helper.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Core.Timers;

public class DotaTimers : ObservableCollection<DotaTimer>
{
    public DotaTimers(IOptions<Settings> settings)
    {
        var timers = new List<DotaTimer>();

        foreach (var item in settings.Value.Timers.Where(x => x.IsEnabled))
        {
            timers.Add(new DotaTimer(
                item.Label, 
                ParseTimeSpan(item.First), 
                ParseTimeSpan(item.Interval), 
                ParseTimeSpan(item.Reminder), 
                item.AudioFile,  
                item.IsManualReset, 
                item.Speech, 
                item.IsTts));
        }
        
        foreach (var timer in timers.OrderBy(x => x.Interval))
        {
            Add(timer);
        }
    }

    private TimeSpan ParseTimeSpan(string x)
    {
        return TimeSpan.ParseExact(x, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
    }
}