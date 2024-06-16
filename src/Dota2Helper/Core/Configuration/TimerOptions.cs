using System;
using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class TimerOptions
{
    [JsonRequired]
    public required string Speech { get; set; }
    
    [JsonRequired]
    public required string Label { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan First { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan Interval { get; set; }
    
    [JsonRequired]
    [JsonConverter(typeof(TimeSpanConverter))]
    public required TimeSpan Reminder { get; set; }
    
    [JsonRequired]
    public required string AudioFile { get; set; }
    
    [JsonRequired]
    public required bool IsManualReset { get; set; }
    
    [JsonRequired]
    public required bool IsEnabled { get; set; }
    
    [JsonRequired]
    public required bool IsTts { get; set; }
}