using System;
using System.Text.Json.Serialization;

namespace Dota2Helper.Features.Settings;

public class DotaTimer
{
    [JsonPropertyName(nameof(IsEnabled))]
    public required bool IsEnabled { get; set; }

    [JsonPropertyName(nameof(IsManualReset))]
    public required bool IsManualReset { get; set; }

    [JsonPropertyName(nameof(IsMuted))]
    public required bool IsMuted { get; set; }

    [JsonPropertyName(nameof(IsInterval))]
    public required bool IsInterval { get; set; }

    [JsonPropertyName(nameof(Name))]
    public required string Name { get; set; }

    [JsonPropertyName(nameof(AudioFile))]
    public string? AudioFile { get; set; }
    
    /// <summary>
    ///  e.g The first spawn at 15 minutes but then every 10 minutes thereafter (using Interval)
    /// </summary>
    [JsonPropertyName(nameof(Offset))]
    public TimeSpan? Offset { get; set; }

    /// <summary>
    ///  The interval between each event
    /// </summary>
    [JsonPropertyName(nameof(Interval))]
    public required TimeSpan Interval { get; set; }

    /// <summary>
    /// e.g Reminder 1 minute before each event
    /// </summary>
    [JsonPropertyName(nameof(RemindBefore))]
    public TimeSpan? RemindBefore { get; set; }

    /// <summary>
    /// e.g Stop showing after 35 minutes, when it's less relevant
    /// </summary>
    [JsonPropertyName(nameof(Visibility))]
    public Visibility Visibility { get; set; } = new();
}

public class Visibility
{
    [JsonPropertyName(nameof(ShowAfter))]
    public TimeSpan? ShowAfter { get; set; }

    [JsonPropertyName(nameof(HideAfter))]
    public TimeSpan? HideAfter { get; set; }
}