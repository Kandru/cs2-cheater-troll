using CheaterTroll.Enums;
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
            get => DefaultMode.ToString(); set => DefaultMode = Enum.TryParse(value, true, out InvisibleEnemiesMode result) ? result : InvisibleEnemiesMode.Full;
        }
    }


    public class InvisibleEnemiesPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        // mode
        [JsonIgnore] public InvisibleEnemiesMode Mode { get; set; } = InvisibleEnemiesMode.Full;
        [JsonPropertyName("mode")]
        public string ModeString
        {
            get => Mode.ToString(); set => Mode = Enum.TryParse(value, true, out InvisibleEnemiesMode result) ? result : InvisibleEnemiesMode.Full;
        }
        // maximum distance to be invisible (in-game units)
        [JsonPropertyName("mode_distance_maximum_distance")] public int Distance { get; set; } = 750;
    }
}