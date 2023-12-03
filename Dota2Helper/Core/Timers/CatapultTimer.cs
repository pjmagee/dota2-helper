using System;

namespace Dota2Helper.Core;

public class CatapultTimer : DotaTimer
{
    public CatapultTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Catapult", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Catapult.mp3";
    }
}