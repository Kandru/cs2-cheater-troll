using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using AntiDLL.API;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "Cheater Troll";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private static PluginCapability<IAntiDLL> AntiDLL { get; } = new PluginCapability<IAntiDLL>("AntiDLL");
        private Dictionary<CCSPlayerController, List<string>> _cheaters = new();
        Random _random = new Random(Guid.NewGuid().GetHashCode());

        public override void Load(bool hotReload)
        {
            // initialize configuration
            ReloadConfigFromDisk();
            // register listeners
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            InitializeInvisibleEnemies();
            // register detection
            try
            {
                IAntiDLL? antidll = AntiDLL.Get();
                if (antidll != null) antidll.OnDetection += this.OnDetection;
            }
            catch { }
            // print message if hot reload
            if (hotReload)
            {
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            ResetInvisibleEnemies();
            // remove detection
            try
            {
                IAntiDLL? antidll = AntiDLL.Get();
                if (antidll != null) antidll.OnDetection -= this.OnDetection;
            }
            catch { }
            Console.WriteLine(Localizer["core.unload"]);
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid!;
            if (!_cheaters.ContainsKey(player)) return HookResult.Continue;
            _cheaters.Remove(player);
            return HookResult.Continue;
        }

        private void OnDetection(CCSPlayerController player, string eventName)
        {
            if (_cheaters.ContainsKey(player)) return;
            // statically add invisible enemy punishment for now
            _cheaters.Add(player, ["invisible_enemies"]);
            Console.WriteLine(Localizer["core.hotreload"].Value.Replace(
                "{message}",
                $"{player.PlayerName} -> {eventName}"
            ));
        }
    }
}
