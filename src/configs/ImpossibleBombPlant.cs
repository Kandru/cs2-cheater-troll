using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class ImpossibleBombPlantConfig
    {
    }


    public class ImpossibleBombPlantPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}