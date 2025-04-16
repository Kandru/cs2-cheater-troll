using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class GrenadeSelfDamageConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // enabled for he grenades
        [JsonPropertyName("enable_hegrenades")] public bool EnableHEGrenades { get; set; } = true;
        // enabled for molotovs
        [JsonPropertyName("enable_molotovs")] public bool EnableMolotovs { get; set; } = true;
        // enabled for smoke grenades
        [JsonPropertyName("enable_flashbangs")] public bool EnableFlashBangs { get; set; } = true;
        // flash bang duration
        [JsonPropertyName("flashbang_duration")] public float FlashBangDuration { get; set; } = 5.0f;
    }


    public class GrenadeSelfDamagePlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}