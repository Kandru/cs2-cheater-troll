using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class ImpossibleBombPlantConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }


    public class ImpossibleBombPlantPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
    }
}