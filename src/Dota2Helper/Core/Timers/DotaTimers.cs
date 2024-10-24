using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dota2Helper.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Dota2Helper.Core.Timers;

public class DotaTimers : ObservableCollection<DotaTimer>
{
    public DotaTimers(IOptions<Settings> settings)
    {
        var timers = new List<DotaTimer>();

        foreach (var item in settings.Value.Timers)
        {
            timers.Add(new DotaTimer(
                item.Label, 
                item.First, 
                item.Interval, 
                item.Reminder, 
                item.AudioFile,  
                item.IsManualReset, 
                item.Speech, 
                item.IsTts,
                item.IsSoundEnabled,
                item.IsEnabled));
        }
        
        foreach (var timer in timers.OrderBy(x => x.Interval))
        {
            Add(timer);
        }
    }

    public void Do(Action<DotaTimer> func)
    {   
        foreach (var timer in this)
        {
            func(timer);
        }
    }
}