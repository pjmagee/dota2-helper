using System;

namespace Dota2Helper.Core;

public class LotusTimer : DotaTimer
{
    public LotusTimer(TimeSpan first, TimeSpan interval, TimeSpan reminderTime) : base("Lotus", first, interval, reminderTime, "audio/Lotus.mp3")
    {
        
    }
}