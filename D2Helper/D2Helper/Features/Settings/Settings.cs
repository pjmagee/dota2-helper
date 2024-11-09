using System.Collections.Generic;
using System.Text.Json.Serialization;
using D2Helper.Features.Timers;

namespace D2Helper.Features.Settings;

[JsonSerializable(typeof(Settings))]
public class Settings
{
    [JsonPropertyName("Timers")]
    public List<DotaTimer> Timers { get; set; } = [];

    [JsonPropertyName("Volume")]
    public double Volume { get; set; } = 50;

    [JsonPropertyName("Mode"), JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeProviderStrategy Mode { get; set; } = TimeProviderStrategy.Auto;

    [JsonPropertyName("DemoMuted")]
    public bool DemoMuted { get; set; } = true;
}