using System;
using System.Collections.Generic;

namespace Dota2Helper.Core.Configuration;

public class Settings
{
    public List<TimerOptions> Timers { get; set; } = new();
    public Uri Address { get; set; } = new("http://localhost:4001/");
}