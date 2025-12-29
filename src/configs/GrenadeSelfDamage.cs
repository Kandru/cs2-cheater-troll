using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class GrenadeSelfDamageConfig
    {
        // enable or disable globally (load plug-in or not)
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // enable or disable for a new cheater by default
        [JsonPropertyName("enabled_for_new_cheater")] public bool DefaultEnabled { get; set; } = false;
        // flash bang duration
        [JsonPropertyName("flashbang_duration")] public float FlashBangDuration { get; set; } = 5.0f;
    }

    public class GrenadeSelfDamagePlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        // enabled for he grenades
        [JsonPropertyName("enable_hegrenades")] public bool EnableHEGrenades { get; set; } = true;
        // enabled for molotovs
        [JsonPropertyName("enable_molotovs")] public bool EnableMolotovs { get; set; } = true;
        // enabled for smoke grenades
        [JsonPropertyName("enable_flashbangs")] public bool EnableFlashBangs { get; set; } = true;
    }
}