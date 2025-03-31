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
            InitializeInvisibleEnemies();
            // print message if hot reload
            if (hotReload)
            {
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
