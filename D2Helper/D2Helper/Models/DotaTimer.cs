using System;
using System.Text.Json.Serialization;

namespace D2Helper.Models;

public class DotaTimer
{
    [JsonPropertyName("IsEnabled")]
    public required bool IsEnabled { get; set; }

    [JsonPropertyName("IsManualReset")]
    public required bool IsManualReset { get; set; }

    [JsonPropertyName("IsMuted")]
    public required bool IsMuted { get; set; }

    [JsonPropertyName("IsInterval")]
    public required bool IsInterval { get; set; }

    [JsonPropertyName("Name")]
    public required string Name { get; set; }

    [JsonPropertyName("Speech")]
    public string? Speech { get; set; }

    [JsonPropertyName("AudioFile")]
    public string? AudioFile { get; set; }

    [JsonPropertyName("Time")]
    public required TimeSpan Time { get; set; }

    [JsonPropertyName("RemindAt")]
    public TimeSpan? RemindAt { get; set; }

    [JsonPropertyName("HideAfter")]
    public TimeSpan? HideAfter { get; set; }

    [JsonPropertyName("ShowAfter")]
    public TimeSpan? ShowAfter { get; set; }
}