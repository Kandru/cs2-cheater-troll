using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class PlayerGlowConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("color")] public string Color { get; set; } = "#ff00a2";
    }

    public class PlayerGlowPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}