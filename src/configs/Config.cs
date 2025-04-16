using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class CheaterConfig
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesPlayerConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesPlayerConfig();
        [JsonPropertyName("grenade_self_damage")] public GrenadeSelfDamagePlayerConfig GrenadeSelfDamage { get; set; } = new GrenadeSelfDamagePlayerConfig();
        [JsonPropertyName("impossible_bomb_plant")] public ImpossibleBombPlantPlayerConfig ImpossibleBombPlant { get; set; } = new ImpossibleBombPlantPlayerConfig();
        [JsonPropertyName("random_player_sounds")] public RandomPlayerSoundsPlayerConfig RandomPlayerSounds { get; set; } = new RandomPlayerSoundsPlayerConfig();
        [JsonPropertyName("always_door_closed")] public AlwaysDoorClosedPlayerConfig AlwaysDoorClosed { get; set; } = new AlwaysDoorClosedPlayerConfig();
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // plug-in InvisibleEnemies
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesConfig();
        // plug-in GrenadeSelfDamage
        [JsonPropertyName("grenade_self_damage")] public GrenadeSelfDamageConfig GrenadeSelfDamage { get; set; } = new GrenadeSelfDamageConfig();
        // plug-in ImpossibleBombPlant
        [JsonPropertyName("impossible_bomb_plant")] public ImpossibleBombPlantConfig ImpossibleBombPlant { get; set; } = new ImpossibleBombPlantConfig();
        // plug-in RandomPlayerSounds
        [JsonPropertyName("random_player_sounds")] public RandomPlayerSoundsConfig RandomPlayerSounds { get; set; } = new RandomPlayerSoundsConfig();
        // plug-in AlwaysDoorClosed
        [JsonPropertyName("always_door_closed")] public AlwaysDoorClosedConfig AlwaysDoorClosed { get; set; } = new AlwaysDoorClosedConfig();
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
