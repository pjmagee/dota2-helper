using System;

namespace Dota2Helper.Core;

public class StackingTimer : DotaTimer
{
    public StackingTimer(TimeSpan first, TimeSpan interval, TimeSpan reminder) : base("Stack", first, interval, reminder, "audio/Stack.mp3")
    {
        
    }
}