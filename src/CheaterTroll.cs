﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "Cheater Troll";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private Dictionary<string, CheaterConfig> _onlineCheaters = [];

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
                        _onlineCheaters.Add(entry.NetworkIDString, cheaterConfig);
                    }
                }
                // initialize listeners if cheaters are online
                InitializeListener();
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // reset plug-ins
            ResetInvisibleEnemies();
            ResetGrenadeSelfDamage();
            ResetImpossibleBombPlant();
            ResetRandomPlayerSounds();
            ResetAlwaysDoorClosed();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private void InitializeListener()
        {
            bool enableInvisibleEnemies = false;
            bool enableGrenadeSelfDamage = false;
            bool enableImpossibleBombPlant = false;
            bool enableRandomPlayerSounds = false;
            bool enableAlwaysDoorClosed = false;
            // check if cheaters have certain features enabled
            foreach (KeyValuePair<string, CheaterConfig> entry in _onlineCheaters)
            {
                if (entry.Value.InvisibleEnemies.Enabled) enableInvisibleEnemies = true;
                if (entry.Value.GrenadeSelfDamage.Enabled) enableGrenadeSelfDamage = true;
                if (entry.Value.ImpossibleBombPlant.Enabled) enableImpossibleBombPlant = true;
                if (entry.Value.RandomPlayerSounds.Enabled) enableRandomPlayerSounds = true;
                if (entry.Value.AlwaysDoorClosed.Enabled) enableAlwaysDoorClosed = true;
            }
            // enable invisible enemies
            if (enableInvisibleEnemies) InitializeInvisibleEnemies();
            // enable grenade self damage
            if (enableGrenadeSelfDamage) InitializeGrenadeSelfDamage();
            // enable impossible bomb plant
            if (enableImpossibleBombPlant) InitializeImpossibleBombPlant();
            // enable random player sounds
            if (enableRandomPlayerSounds) InitializeRandomPlayerSounds();
            // enable always door closed
            if (enableAlwaysDoorClosed) InitializeAlwaysDoorClosed();
        }

        private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || string.IsNullOrEmpty(player.NetworkIDString)
                || !Config.Cheater.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            // add cheater to active cheaters if in config
            _onlineCheaters.Add(
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
                || string.IsNullOrEmpty(player.NetworkIDString)
                || !_onlineCheaters.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            // remove cheater from active cheaters (but keep in config)
            _onlineCheaters.Remove(player.NetworkIDString);
            return HookResult.Continue;
        }
    }
}
