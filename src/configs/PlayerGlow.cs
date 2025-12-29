using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class PlayerGlowConfig
    {
        // enable or disable globally (load plug-in or not)
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // enable or disable for a new cheater by default
        [JsonPropertyName("enabled_for_new_cheater")] public bool DefaultEnabled { get; set; } = false;
        [JsonPropertyName("color")] public string Color { get; set; } = "#ff00a2";
    }

    public class PlayerGlowPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}