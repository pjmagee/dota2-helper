using System.Text.Json.Serialization;

namespace Dota2Helper.Core;

public class GameState
{
    [JsonPropertyName("map")]
    public required Map Map { get; set; }
}