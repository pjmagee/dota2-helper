using System;

namespace Dota2Helper.Core;

public class RoshanTimer : DotaTimer
{
    public RoshanTimer(TimeSpan first, TimeSpan interval, TimeSpan reminder) : base("Roshan", first, interval, reminder, "audio/Roshan.mp3")
    {
        IsManualReset = true;
    }
}