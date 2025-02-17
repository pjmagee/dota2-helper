using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dota2Helper.Features.Settings;

public class Profile
{
    [JsonPropertyName("Name")]
    public required string Name { get; set; }

    [JsonPropertyName("Timers")]
    public List<DotaTimer> Timers { get; set; } = [];
}