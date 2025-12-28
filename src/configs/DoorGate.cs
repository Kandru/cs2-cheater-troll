using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class DoorGateConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // sound to play on door close
        [JsonPropertyName("sound")] public string Sound { get; set; } = "Saysound.Knock";
        // speed of door open
        [JsonPropertyName("speed")] public float Speed { get; set; } = 60f;
        // time after which the door closes
        [JsonPropertyName("delay")] public float Delay { get; set; } = 0.1f;
    }

    public class DoorGatePlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // don't allow to open doors
        [JsonPropertyName("prevent_open")] public bool PreventOpen { get; set; } = true;
        // close automatically when nearby
        [JsonPropertyName("close_nearby_doors")] public bool CloseNearby { get; set; } = true;
        // door close distance
        [JsonPropertyName("door_close_distance")] public float DoorCloseDistance { get; set; } = 200f;
    }
}