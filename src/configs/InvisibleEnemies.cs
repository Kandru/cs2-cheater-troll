using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class InvisibleEnemiesConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonIgnore] public InvisibleEnemiesMode DefaultMode { get; set; } = InvisibleEnemiesMode.Full;
        [JsonPropertyName("mode")]
        public string DefaultModeString
        {
            get => DefaultMode.ToString();
            set
            {
                if (Enum.TryParse<InvisibleEnemiesMode>(value, true, out var result))
                    DefaultMode = result;
                else
                    DefaultMode = InvisibleEnemiesMode.Distance;
            }
        }
        //// mode "distance
        // minimum distance to be invisible (in-game units)
        [JsonPropertyName("mode-distance-distance")] public int Distance { get; set; } = 750;
        //// mode "random"
        // percentage of enemies to be invisible at any given time
        [JsonPropertyName("mode-random-percentage")] public int RandomPercentage { get; set; } = 75;
        // minimum time to change invisible enemies in seconds
        [JsonPropertyName("mode-random-min_time")] public int RandomMinTime { get; set; } = 2;
        // maximum time to change invisible enemies in seconds
        [JsonPropertyName("mode-random-max_time")] public int RandomMaxTime { get; set; } = 5;
    }


    public class InvisibleEnemiesPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // mode
        [JsonIgnore] public InvisibleEnemiesMode Mode { get; set; } = InvisibleEnemiesMode.Full;
        [JsonPropertyName("mode")]
        public string ModeString
        {
            get => Mode.ToString();
            set
            {
                if (Enum.TryParse<InvisibleEnemiesMode>(value, true, out var result))
                    Mode = result;
                else
                    Mode = InvisibleEnemiesMode.Full;
            }
        }
    }
}