using System.Text.Json.Serialization;

namespace Dota2Helper.Features.Http.Serialisation;

public class GameState
{
    [JsonPropertyName("map")]
    public Map? Map { get; set; }
}