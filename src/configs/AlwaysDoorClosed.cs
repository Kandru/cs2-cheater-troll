using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class AlwaysDoorClosedConfig
    {
        // sound to play on door close
        [JsonPropertyName("sound")] public string Sound { get; set; } = "Saysound.Knock";
        // speed of door open
        [JsonPropertyName("speed")] public float Speed { get; set; } = 30f;
        // time after which the door closes
        [JsonPropertyName("delay")] public float Delay { get; set; } = 1.5f;
    }

    public class AlwaysDoorClosedPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
    }
}