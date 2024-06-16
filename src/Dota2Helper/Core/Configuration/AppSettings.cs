using System.Text.Json.Serialization;

namespace Dota2Helper.Core.Configuration;

public class AppSettings
{
    [JsonPropertyName("Settings")]
    public Settings Settings { get; set; }
}