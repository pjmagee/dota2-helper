using System;

namespace Dota2Helper.Core;

public class StackingTimer : DotaTimer
{
    public StackingTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Stack", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Stack.mp3";
    }
}