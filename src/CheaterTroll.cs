using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "Cheater Troll";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private Dictionary<string, CheaterConfig> _cheaters = new();
        Random _random = new Random(Guid.NewGuid().GetHashCode());

        public override void Load(bool hotReload)
        {
            // register listeners
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // check for hot reload
            if (hotReload)
            {
                // check if cheaters are on our server if hot-reloaded
                foreach (CCSPlayerController entry in Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && !p.IsHLTV))
                {
                    if (Config.Cheater.TryGetValue(entry.NetworkIDString, out CheaterConfig? cheaterConfig))
                    {
                        _cheaters.Add(entry.NetworkIDString, cheaterConfig);
                    }
                }
                // initialize listeners
                InitializeListener();
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            ResetInvisibleEnemies();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private void InitializeListener()
        {
            bool enableInvisibleENemies = false;
            // check if cheaters have certain features enabled
            foreach (KeyValuePair<string, CheaterConfig> entry in _cheaters)
            {
                if (entry.Value.InvisibleEnemies) enableInvisibleENemies = true;
            }
            // enable invisible enemies
            if (enableInvisibleENemies) InitializeInvisibleEnemies();
        }

        private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || !Config.Cheater.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            // add cheater to active cheaters if in config
            _cheaters.Add(
                player.NetworkIDString,
                Config.Cheater[player.NetworkIDString]);
            // initialize listener
            InitializeListener();
            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || !_cheaters.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            // remove cheater from active cheaters (but keep in config)
            _cheaters.Remove(player.NetworkIDString);
            return HookResult.Continue;
        }
    }
}
