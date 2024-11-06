using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace D2Helper.Models;

[JsonSerializable(typeof(Settings))]
public class Settings
{
    [JsonPropertyName("Timers")]
    public List<DotaTimer> Timers { get; set; } = [];

    [JsonPropertyName("Volume")]
    public double Volume { get; set; } = 50;

    [JsonPropertyName("Mode"), JsonConverter(typeof(JsonStringEnumConverter))]
    public GameStateStrategy Mode { get; set; } = GameStateStrategy.Auto;

    [JsonPropertyName("DemoMuted")]
    public bool DemoMuted { get; set; } = true;
}