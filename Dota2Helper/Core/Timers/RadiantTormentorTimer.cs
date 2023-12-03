using System;

namespace Dota2Helper.Core;

public class RadiantTormentorTimer : DotaTimer
{
    public RadiantTormentorTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Tormentor (R)", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Roshan.mp3";
        base.IsManualReset = true;
    }
}