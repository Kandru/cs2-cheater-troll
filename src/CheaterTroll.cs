using CounterStrikeSharp.API.Core;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "Cheater Troll";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        Random _random = new Random(Guid.NewGuid().GetHashCode());

        public override void Load(bool hotReload)
        {
            // initialize configuration
            LoadConfig();
            UpdateConfig();
            SaveConfig();
            // register listeners
            // print message if hot reload
            if (hotReload)
            {
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            Console.WriteLine(Localizer["core.unload"]);
        }
    }
}
