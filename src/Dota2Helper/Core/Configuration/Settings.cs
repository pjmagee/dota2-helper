using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class Settings
{
    [JsonPropertyName("Timers")]
    public List<TimerOptions> Timers { get; set; }
    
    [JsonPropertyName("Address")]
    public required Uri Address { get; set; } = new("http://localhost:4001/");
}