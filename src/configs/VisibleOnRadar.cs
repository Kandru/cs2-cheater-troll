using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class VisibleOnRadarConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }

    public class VisibleOnRadarPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}