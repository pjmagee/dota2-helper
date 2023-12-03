using System;

namespace Dota2Helper.Core;

public class BountyTimer : DotaTimer
{
    public BountyTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Bounty", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Bounty.mp3";
    }
}