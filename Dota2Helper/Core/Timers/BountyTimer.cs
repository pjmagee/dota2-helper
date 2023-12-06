using System;

namespace Dota2Helper.Core;

public class BountyTimer : DotaTimer
{
    public BountyTimer(
        TimeSpan first, 
        TimeSpan interval, 
        TimeSpan reminder) : base("Bounty", first, interval, reminder, "audio/Bounty.mp3")
    {
        
    }
}