using System;
using System.Collections.Generic;
using Avalonia.Collections;

namespace Dota2Helper.Core;

public class FakeDotaTimers : AvaloniaDictionary<string, DotaTimer>
{
    private static List<DotaTimer> Default =>
    [
        new DotaTimer("Stack", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(30), "audio/Stack.mp3"),
        new DotaTimer("Catapult", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(3), TimeSpan.FromSeconds(30), "audio/Catapult.mp3"),
        new DotaTimer("Bounty", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(30), "audio/Bounty.mp3"),
        new DotaTimer("Wisdom", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(30), "audio/Wisdom.mp3"),
        new DotaTimer("Roshan", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(8), TimeSpan.FromSeconds(30), "audio/Roshan.mp3"),
        new DotaTimer("Outpost", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(30), "audio/Outpost.mp3"), 
        new DotaTimer("Power", TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(30), "audio/Power.mp3")
    ];
    
    public FakeDotaTimers()
    {
        foreach (var item in Default)
        {
            Add(item.Label, item);
        }
    }
}