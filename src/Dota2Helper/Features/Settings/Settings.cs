using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Features.Settings;

public class Settings
{
    [JsonPropertyName("$schema")]

    public string Schema { get; set; } = "./appsettings.schema.json";

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

    [JsonPropertyName("Theme")]
    public string Theme { get; set; } = "Light";
}