using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace CheaterTroll
{
    public class CheaterConfig
    {
        [JsonPropertyName("invisible_enemies")] public bool InvisibleEnemies { get; set; } = false;
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
        public required PluginConfig Config { get; set; }

        private void ReloadConfigFromDisk()
        {
            try
            {
                // load config from disk
                Config.Reload();
                // save config to disk
                Config.Update();
            }
            catch (Exception e)
            {
                string message = Localizer["core.error"].Value.Replace("{error}", e.Message);
                // log error
                Console.WriteLine(message);
            }
        }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Console.WriteLine(Localizer["core.config"]);
        }
    }
}
