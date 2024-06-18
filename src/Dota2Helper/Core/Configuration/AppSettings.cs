using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class AppSettings
{
    [JsonPropertyName("Settings")]
    public required Settings Settings { get; set; }
}