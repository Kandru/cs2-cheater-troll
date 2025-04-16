using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class InvisibleEnemiesConfig
    {
        //// mode "distance
        // minimum amount of enemies to be enabled
        [JsonPropertyName("mode-distance-min_enemies")] public int MinEnemies { get; set; } = 1;
        // minimum distance to be invisible
        [JsonPropertyName("mode-distance-min_distance")] public int MinDistance { get; set; } = 100;
        //// mode "random"
        // minimum amount of enemies to be enabled
        [JsonPropertyName("mode-random-min_enemies")] public int MinEnemiesRandom { get; set; } = 2;
        // percentage of enemies to be invisible at any given time
        [JsonPropertyName("mode-random-percentage")] public int Percentage { get; set; } = 50;
        // minimum time to change invisible enemies in seconds
        [JsonPropertyName("mode-random-min_time")] public int MinTime { get; set; } = 5;
        // maximum time to change invisible enemies in seconds
        [JsonPropertyName("mode-random-max_time")] public int MaxTime { get; set; } = 10;
    }


    public class InvisibleEnemiesPlayerConfig
    {
        // enabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // mode
        [JsonPropertyName("mode")] public InvisibleEnemiesMode Mode { get; set; } = InvisibleEnemiesMode.Distance;
    }

    public class CheaterConfig
    {
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesPlayerConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesPlayerConfig();
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // plug-in InvisibleEnemies
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesConfig();
        // list of cheaters
        [JsonPropertyName("cheater")] public Dictionary<string, CheaterConfig> Cheater { get; set; } = [];
    }

    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            // update configuration with latest plugin changes
            Config.Update();
            Console.WriteLine(Localizer["core.config"]);
        }
    }
}
