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

    [JsonPropertyName(nameof(Time))]
    public required TimeSpan Time { get; set; }

    [JsonPropertyName(nameof(RemindAt))]
    public TimeSpan? RemindAt { get; set; }

    [JsonPropertyName(nameof(StopAfter))]
    public TimeSpan? StopAfter { get; set; }

    [JsonPropertyName(nameof(StartAfter))]
    public TimeSpan? StartAfter { get; set; }
}