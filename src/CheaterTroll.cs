using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using CheaterTroll.Enums;
using CheaterTroll.Plugins;
using CheaterTroll.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "Cheater Troll";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";


        // use a dictionary for the connected players to save information for further use:
        // - the player name on connection to avoid fast name-changing hacks and allow the player to be identified properly
        // entries: name, steam_id, timestamp (CurrentTime)
        private readonly Dictionary<CCSPlayerController, Dictionary<PlayerData, string>> _connectedPlayers = [];
        private readonly List<PluginBlueprint> _plugins = [];

        public override void Load(bool hotReload)
        {
            // register listeners
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // register plugins
            InitializeModules(hotReload);
            // check for hot reload
            if (hotReload)
            {
                UpdatePlayerInfos();
                EnableAllPlayerCheats();
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // unload plugins
            DestroyModules();
            // write config to disk
            Config.Update();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private void OnMapStart(string mapName)
        {
            // initialize plugins
            InitializeModules();
        }

        private void OnMapEnd()
        {
            DestroyModules();
            _clientConsoleMenu.ClearAllStates();
            Config.Update();
        }

        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid)
            {
                return HookResult.Continue;
            }
            // update player info for future usage
            UpdatePlayerInfo(player);
            // check if this is a cheater
            if (string.IsNullOrEmpty(player.SteamID.ToString())
                || !Config.Cheater.ContainsKey(player.SteamID.ToString()))
            {
                return HookResult.Continue;
            }
            // enable player cheats (if any)
            EnablePlayerCheats(player);
            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid)
            {
                return HookResult.Continue;
            }

            // remove player info
            RemovePlayerInfo(player);
            // remove client console menu state
            _clientConsoleMenu.RemovePlayerState(player);
            // check if player is a cheater
            if (!CheckIfCheater(player))
            {
                return HookResult.Continue;
            }
            // remove plugins for player
            RemovePluginsForUser(player);
            return HookResult.Continue;
        }

        private void InitializeModules(bool IshotReloaded = false)
        {
            if (_plugins.Count > 0)
            {
                return;
            }

            // plug-in invisible enemies
            if (Config.Plugins.InvisibleEnemies.Enabled)
            {
                _plugins.Add(new InvisibleEnemies(Config, Localizer, IshotReloaded));
            }

            // plug-in random player sounds
            if (Config.Plugins.RandomPlayerSounds.Enabled)
            {
                _plugins.Add(new RandomPlayerSounds(Config, Localizer, IshotReloaded));
            }

            // plug-in grenade self damage
            if (Config.Plugins.GrenadeSelfDamage.Enabled)
            {
                _plugins.Add(new GrenadeSelfDamage(Config, Localizer, IshotReloaded));
            }

            // plug-in impossible bomb plant
            if (Config.Plugins.ImpossibleBombPlant.Enabled)
            {
                _plugins.Add(new ImpossibleBombPlant(Config, Localizer, IshotReloaded));
            }

            // plug-in door gate
            if (Config.Plugins.DoorGate.Enabled)
            {
                _plugins.Add(new DoorGate(Config, Localizer, IshotReloaded));
            }

            // plug-in damage control
            if (Config.Plugins.DamageControl.Enabled)
            {
                _plugins.Add(new DamageControl(Config, Localizer, IshotReloaded));
            }

            // plug-in Glow
            if (Config.Plugins.PlayerGlow.Enabled)
            {
                _plugins.Add(new PlayerGlow(Config, Localizer, IshotReloaded));
            }

            // plug-in visible on radar
            if (Config.Plugins.VisibleOnRadar.Enabled)
            {
                _plugins.Add(new VisibleOnRadar(Config, Localizer, IshotReloaded));
            }

            // plug-in heavy knife
            if (Config.Plugins.HeavyKnife.Enabled)
            {
                _plugins.Add(new HeavyKnife(Config, Localizer, IshotReloaded));
            }

            // plug-in announce position
            if (Config.Plugins.AnnouncePosition.Enabled)
            {
                _plugins.Add(new AnnouncePosition(Config, Localizer, IshotReloaded));
            }

            // register listeners
            RegisterListeners();
            RegisterEventHandlers();
            RegisterUserMessageHooks();
        }

        private void DestroyModules()
        {
            // deregister listeners
            DeregisterListeners();
            DeregisterEventHandlers();
            DeregisterUserMessageHooks();
            // destroy all cosmetics modules
            foreach (PluginBlueprint module in _plugins)
            {
                module.Destroy();
            }
            _plugins.Clear();
        }

        private void RegisterListeners()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Initializing listener for module {module.GetType().Name}");
                foreach (string listenerName in module.Listeners)
                {
                    //DebugPrint($"- {listenerName}");
                    DynamicHandlers.RegisterModuleListener(this, listenerName, module);
                }
            }
        }

        private void DeregisterListeners()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Destroying listener for module {module.GetType().Name}");
                foreach (string listenerName in module.Listeners)
                {
                    //DebugPrint($"- {listenerName}");
                    DynamicHandlers.DeregisterModuleListener(this, listenerName, module);
                }
            }
        }

        private void RegisterEventHandlers()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Initializing event handlers for module {module.GetType().Name}");
                foreach (string eventName in module.Events)
                {
                    //DebugPrint($"- {eventName}");
                    DynamicHandlers.RegisterModuleEventHandler(this, eventName, module);
                }
            }
        }

        private void DeregisterEventHandlers()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Destroying event handlers for module {module.GetType().Name}");
                foreach (string eventName in module.Events)
                {
                    //DebugPrint($"- {eventName}");
                    DynamicHandlers.DeregisterModuleEventHandler(this, eventName, module);
                }
            }
        }

        private void RegisterUserMessageHooks()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Registering user messages for module {module.GetType().Name}");
                foreach ((int userMessageId, HookMode hookMode) in module.UserMessages)
                {
                    //DebugPrint($"- UserMessage ID: {userMessageId}, HookMode: {hookMode}");
                    DynamicHandlers.RegisterUserMessageHook(this, userMessageId, module, hookMode);
                }
            }
        }

        private void DeregisterUserMessageHooks()
        {
            foreach (PluginBlueprint module in _plugins)
            {
                //DebugPrint($"Deregistering user messages for module {module.GetType().Name}");
                foreach ((int userMessageId, HookMode hookMode) in module.UserMessages)
                {
                    //DebugPrint($"- UserMessage ID: {userMessageId}, HookMode: {hookMode}");
                    DynamicHandlers.DeregisterUserMessageHook(this, userMessageId, module, hookMode);
                }
            }
        }

        private void RemovePluginsForUser(CCSPlayerController? player)
        {
            if (player == null
                || !player.IsValid)
            {
                return;
            }
            // remove the plugins for the player (if any)
            foreach (PluginBlueprint plugin in _plugins)
            {
                if (plugin._players.ContainsKey(player))
                {
                    plugin.Remove(player);
                }
            }
        }

        private void UpdatePlayerInfos()
        {
            // update all player infos
            foreach (CCSPlayerController entry in Utilities.GetPlayers().Where(static p => !p.IsBot && !p.IsHLTV))
            {
                UpdatePlayerInfo(entry);
            }
        }

        private void UpdatePlayerInfo(CCSPlayerController player)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV)
            {
                return;
            }
            // add player to dictionary
            if (!_connectedPlayers.ContainsKey(player))
            {
                _connectedPlayers.Add(player, []);
            }
            // update player data in dictionary
            _connectedPlayers[player][PlayerData.PLAYER_NAME] = player.PlayerName;
            _connectedPlayers[player][PlayerData.STEAM_ID] = player.SteamID.ToString();
            _connectedPlayers[player][PlayerData.TIMESTAMP] = Server.CurrentTime.ToString();
        }

        private void RemovePlayerInfo(CCSPlayerController player)
        {
            if (player == null
                || !player.IsValid)
            {
                return;
            }
            // remove player from dictionary
            if (_connectedPlayers.ContainsKey(player))
            {
                _ = _connectedPlayers.Remove(player);
            }
        }
    }
}
