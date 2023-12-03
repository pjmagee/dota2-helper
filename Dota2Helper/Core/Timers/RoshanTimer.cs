using System;

namespace Dota2Helper.Core;

public class RoshanTimer : DotaTimer
{
    public RoshanTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Roshan", fromGameStart, interval, reminderTime)
    {
        SoundToPlay = "audio/Roshan.mp3";
        IsManualReset = true;
    }
}