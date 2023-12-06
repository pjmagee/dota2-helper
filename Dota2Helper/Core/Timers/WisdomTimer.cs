using System;

namespace Dota2Helper.Core;

public class WisdomTimer : DotaTimer
{
    public WisdomTimer(TimeSpan first, TimeSpan interval, TimeSpan reminder) : base("Wisdom", first, interval, reminder, "audio/Wisdom.mp3")
    {
        
    }
}