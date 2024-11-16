using System.Collections.Generic;
using System.Text.Json.Serialization;
using D2Helper.Features.Timers;

namespace D2Helper.Features.Settings;

[JsonSerializable(typeof(Settings))]
public class Settings
{
    [JsonPropertyName("Profiles")]
    public List<Profile> Profiles { get; set; } = [];

    [JsonPropertyName("SelectedProfileIdx")]
    public int SelectedProfileIdx { get; set; } = 0;

    [JsonPropertyName("Volume")]
    public double Volume { get; set; } = 50;

    [JsonPropertyName("Mode"), JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeMode Mode { get; set; } = TimeMode.Auto;

    [JsonPropertyName("DemoMuted")]
    public bool DemoMuted { get; set; } = true;
}

[JsonSerializable(typeof(Profile))]
public class Profile
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Timers")]
    public List<DotaTimer> Timers { get; set; } = [];
}