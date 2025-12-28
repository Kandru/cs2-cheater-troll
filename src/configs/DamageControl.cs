using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class DamageControlConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }

    public class DamageControlPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("damage_limit")] public bool DamageLimit { get; set; } = true;
        [JsonPropertyName("max_damage")] public int MaxDamage { get; set; } = 1;
        [JsonPropertyName("self_damage")] public bool SelfDamage { get; set; } = false;
        [JsonPropertyName("self_damage_percentage")] public float SelfDamagePercentage { get; set; } = 1f;
    }
}