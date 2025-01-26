using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Config;

namespace CheaterTroll
{
    public class CheaterConfig
    {
        [JsonPropertyName("health")] public int Health { get; set; } = 100;
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        [JsonPropertyName("cheater")] public Dictionary<string, CheaterConfig> Cheater { get; set; } = new Dictionary<string, CheaterConfig>();
    }

    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public PluginConfig Config { get; set; } = null!;
        private string _configPath = "";

        private void LoadConfig()
        {
            Config = ConfigManager.Load<PluginConfig>("CheaterTroll");
            _configPath = Path.Combine(ModuleDirectory, $"../../configs/plugins/CheaterTroll/CheaterTroll.json");
        }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Console.WriteLine("[CheaterTroll] Initialized map configuration!");
        }

        private void UpdateConfig()
        {
        }

        private void SaveConfig()
        {
            var jsonString = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, jsonString);
        }
    }
}
