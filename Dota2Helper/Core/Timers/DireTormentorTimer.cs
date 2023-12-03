using System;

namespace Dota2Helper.Core;

public class DireTormentorTimer : DotaTimer
{
    public DireTormentorTimer(TimeSpan fromGameStart, TimeSpan interval, TimeSpan reminderTime) : base("Tormentor (D)", fromGameStart, interval, reminderTime)
    {
        this.SoundToPlay = "audio/Tormentor.mp3";
        this.IsManualReset = true;
    }
}