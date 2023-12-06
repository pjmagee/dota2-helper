using System;

namespace Dota2Helper.Core;

public class PowerTimer : DotaTimer
{
    public PowerTimer(TimeSpan first, TimeSpan interval, TimeSpan reminderTime) : base("Power", first, interval, reminderTime, "audio/Power.mp3")
    {
        
    }
}