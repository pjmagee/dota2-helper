using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                label: item.Label,
                first: item.First,
                interval: item.Interval,
                reminder: item.Reminder,
                offset: item.Offset,
                expireAt: item.ExpireAt,
                audioFile: item.AudioFile,
                isManualReset: item.IsManualReset,
                speech: item.Speech,
                isTts: item.IsTts,
                isSoundEnabled: item.IsSoundEnabled,
                isEnabled: item.IsEnabled));
        }
        
        foreach (var timer in timers)
        {
            Add(timer);
        }
    }

    public void Do(Action<DotaTimer> func)
    {   
        foreach (var timer in this)
        {
            try
            {
                func(timer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}