using System;

namespace Dota2Helper.Core;

public class PowerTimer : DotaTimer
{
    public PowerTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Power", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Power.mp3";
    }
}