using System;

namespace Dota2Helper.Core;

public class DireTormentorTimer : DotaTimer
{
    public DireTormentorTimer(TimeSpan first, TimeSpan interval, TimeSpan reminder) : base("Tormentor (D)", first, interval, reminder, "audio/Tormentor.mp3")
    {
        IsManualReset = true;
    }
}