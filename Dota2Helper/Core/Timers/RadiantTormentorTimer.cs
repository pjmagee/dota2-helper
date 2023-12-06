using System;

namespace Dota2Helper.Core;

public class RadiantTormentorTimer : DotaTimer
{
    public RadiantTormentorTimer(TimeSpan first, TimeSpan interval, TimeSpan reminder) : base("Tormentor (R)", first, interval, reminder, "audio/Tormentor.mp3")
    {
        IsManualReset = true;
    }
}