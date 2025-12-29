using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class CheaterConfig
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("always_door_closed")] public DoorGatePlayerConfig DoorGate { get; set; } = new DoorGatePlayerConfig();
        [JsonPropertyName("damage_control")] public DamageControlPlayerConfig DamageControl { get; set; } = new DamageControlPlayerConfig();
        [JsonPropertyName("glow")] public PlayerGlowPlayerConfig PlayerGlow { get; set; } = new PlayerGlowPlayerConfig();
        [JsonPropertyName("grenade_self_damage")] public GrenadeSelfDamagePlayerConfig GrenadeSelfDamage { get; set; } = new GrenadeSelfDamagePlayerConfig();
        [JsonPropertyName("heavy_knife")] public HeavyKnifePlayerConfig HeavyKnife { get; set; } = new HeavyKnifePlayerConfig();
        [JsonPropertyName("impossible_bomb_plant")] public ImpossibleBombPlantPlayerConfig ImpossibleBombPlant { get; set; } = new ImpossibleBombPlantPlayerConfig();
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesPlayerConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesPlayerConfig();
        [JsonPropertyName("random_player_sounds")] public RandomPlayerSoundsPlayerConfig RandomPlayerSounds { get; set; } = new RandomPlayerSoundsPlayerConfig();
        [JsonPropertyName("visible_on_radar")] public VisibleOnRadarPlayerConfig VisibleOnRadar { get; set; } = new VisibleOnRadarPlayerConfig();
    }

    public class PluginsConfig
    {
        [JsonPropertyName("always_door_closed")] public DoorGateConfig DoorGate { get; set; } = new DoorGateConfig();
        [JsonPropertyName("damage_control")] public DamageControlConfig DamageControl { get; set; } = new DamageControlConfig();
        [JsonPropertyName("glow")] public PlayerGlowConfig PlayerGlow { get; set; } = new PlayerGlowConfig();
        [JsonPropertyName("grenade_self_damage")] public GrenadeSelfDamageConfig GrenadeSelfDamage { get; set; } = new GrenadeSelfDamageConfig();
        [JsonPropertyName("heavy_knife")] public HeavyKnifeConfig HeavyKnife { get; set; } = new HeavyKnifeConfig();
        [JsonPropertyName("impossible_bomb_plant")] public ImpossibleBombPlantConfig ImpossibleBombPlant { get; set; } = new ImpossibleBombPlantConfig();
        [JsonPropertyName("invisible_enemies")] public InvisibleEnemiesConfig InvisibleEnemies { get; set; } = new InvisibleEnemiesConfig();
        [JsonPropertyName("random_player_sounds")] public RandomPlayerSoundsConfig RandomPlayerSounds { get; set; } = new RandomPlayerSoundsConfig();
        [JsonPropertyName("visible_on_radar")] public VisibleOnRadarConfig VisibleOnRadar { get; set; } = new VisibleOnRadarConfig();
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // global config
        [JsonPropertyName("global_config")] public PluginsConfig Plugins { get; set; } = new();
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

        private CheaterConfig CreateCheaterConfigWithDefaults()
        {
            CheaterConfig config = new();

            config.InvisibleEnemies.Enabled = Config.Plugins.InvisibleEnemies.DefaultEnabled;
            config.RandomPlayerSounds.Enabled = Config.Plugins.RandomPlayerSounds.DefaultEnabled;
            config.GrenadeSelfDamage.Enabled = Config.Plugins.GrenadeSelfDamage.DefaultEnabled;
            config.ImpossibleBombPlant.Enabled = Config.Plugins.ImpossibleBombPlant.DefaultEnabled;
            config.DoorGate.Enabled = Config.Plugins.DoorGate.DefaultEnabled;
            config.DamageControl.Enabled = Config.Plugins.DamageControl.DefaultEnabled;
            config.PlayerGlow.Enabled = Config.Plugins.PlayerGlow.DefaultEnabled;
            config.VisibleOnRadar.Enabled = Config.Plugins.VisibleOnRadar.DefaultEnabled;

            return config;
        }

        private void DeleteCheaterConfig(CCSPlayerController player)
        {
            if (player == null
                || !player.IsValid)
            {
                return;
            }
            // remove cheater from config
            _ = Config.Cheater.Remove(player.SteamID.ToString());
        }

        private bool CheckIfCheater(CCSPlayerController player)
        {
            string steamId = player.SteamID.ToString();
            if (Config.Cheater.ContainsKey(steamId))
            {
                return true;
            }
            return false;
        }
    }
}
