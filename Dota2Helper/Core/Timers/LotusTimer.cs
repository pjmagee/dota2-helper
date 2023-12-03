using System;

namespace Dota2Helper.Core;

public class LotusTimer : DotaTimer
{
    public LotusTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Lotus", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Lotus.mp3";
    }
}