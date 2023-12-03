using System;

namespace Dota2Helper.Core;

public class WisdomTimer : DotaTimer
{
    public WisdomTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Wisdom", fromGameStart, interval, reminderTime)
    {
        SoundToPlay = "audio/Wisdom.mp3";
    }
}