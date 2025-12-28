using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class RandomPlayerSoundConfig
    {
        // sound name
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        // how often the sound should be played
        [JsonPropertyName("amount")] public int Amount { get; set; } = 1;

        // time in seconds between sounds
        [JsonPropertyName("interval")] public float Interval { get; set; } = 1.0f;
    }

    public class RandomPlayerSoundsConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // random sound list with their parameters
        [JsonPropertyName("sounds")]
        public List<RandomPlayerSoundConfig> Sounds { get; set; } = [
            new RandomPlayerSoundConfig { Name = "Weapon_Knife.Deploy" },
            new RandomPlayerSoundConfig { Name = "Weapon_Knife.Slash" },
            new RandomPlayerSoundConfig { Name = "BaseSmokeEffect.Sound" },
            new RandomPlayerSoundConfig { Name = "HEGrenade.PullPin_Grenade" },
            new RandomPlayerSoundConfig { Name = "Weapon_Taser.Single" },
        ];
        // waiting time after round freeze end
        [JsonPropertyName("wait_time")] public int WaitTime { get; set; } = 5;
        // minimum time between sounds
        [JsonPropertyName("min_time")] public int RandomMinTime { get; set; } = 5;
        // maximum time between sounds
        [JsonPropertyName("max_time")] public int RandomMaxTime { get; set; } = 30;
    }

    public class RandomPlayerSoundsPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
    }
}