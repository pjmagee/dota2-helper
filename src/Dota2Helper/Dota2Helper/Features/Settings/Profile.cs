using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dota2Helper.Features.Timers;

namespace Dota2Helper.Features.Settings;

public class Profile
{
    [JsonPropertyName("Name")]
    public required string Name { get; set; }

    [JsonPropertyName("Timers")]
    public List<DotaTimer> Timers { get; set; } = [];
}