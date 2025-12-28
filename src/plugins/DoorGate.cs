using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CheaterTroll.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class DoorGate : PluginBlueprint
    {
        public override string Description => "Messing with doors";
        public override string ClassName => "DoorGate";
        public override List<string> Listeners => [
            "OnTick"
        ];
        public override List<string> Events => [
            "EventDoorOpen",
            "EventRoundStart"
        ];
        private readonly List<long> _doorsInUse = [];
        private readonly Dictionary<CBaseDoor, Vector> _doorPositions = [];

        public DoorGate(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            if (IshotReloaded)
            {
                GetDoorPositions();
            }
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public void OnTick()
        {
            if (_players.Count == 0
                || _doorPositions.Count == 0)
            {
                return;
            }
            foreach (KeyValuePair<CCSPlayerController, CheaterConfig> kvp in _players.Where(static p => p.Value.DoorGate.CloseNearby))
            {
                CCSPlayerController player = kvp.Key;
                CCSPlayerPawn? playerPawn = player.PlayerPawn?.Value;

                if (playerPawn?.AbsOrigin == null)
                {
                    continue;
                }

                Vector playerOrigin = playerPawn.AbsOrigin;

                foreach (KeyValuePair<CBaseDoor, Vector> doorKvp in _doorPositions)
                {
                    double distance = Math.Round(Vectors.GetDistance(playerOrigin, doorKvp.Value));
                    if (distance >= kvp.Value.DoorGate.DoorCloseDistance - 10
                        && distance <= kvp.Value.DoorGate.DoorCloseDistance
                        && doorKvp.Key != null && doorKvp.Key.IsValid)
                    {
                        CBaseDoor door = doorKvp.Key;
                        door.AcceptInput("Close");
                    }
                }
            }
        }


        public HookResult EventDoorOpen(EventDoorOpen @event, GameEventInfo info)
        {
            long doorIndex = @event.Entindex;
            // get player who opened the door
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !_players[player].DoorGate.PreventOpen
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }

            CBaseDoor? door = Utilities.GetEntityFromIndex<CBaseDoor>((int)doorIndex);
            if (door == null
                || !door.IsValid)
            {
                return HookResult.Continue;
            }
            // open slowly with a random chance
            if (new Random().Next(0, 2) == 0)
            {
                _doorsInUse.Add(doorIndex);
                float oldSpeed = door.Speed;
                door.AcceptInput("SetSpeed", door, door, _globalConfig.Plugins.DoorGate.Speed.ToString());
                _ = new CounterStrikeSharp.API.Modules.Timers.Timer(_globalConfig.Plugins.DoorGate.Delay, () =>
                {
                    // reset door
                    _ = _doorsInUse.Remove(doorIndex);
                    if (door == null || !door.IsValid)
                    {
                        return;
                    }
                    // reset speed
                    door.AcceptInput("SetSpeed", door, door, oldSpeed.ToString());
                    // close again
                    door.AcceptInput("Close");
                });
                return HookResult.Continue;
            }
            else
            {
                door.AcceptInput("Close");
                _ = door.EmitSound(_globalConfig.Plugins.DoorGate.Sound);
            }
            return HookResult.Continue;
        }

        public HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            GetDoorPositions();
            return HookResult.Continue;
        }

        private void GetDoorPositions()
        {
            _doorPositions.Clear();
            // get all door positions from the map and cache them
            foreach (CBaseDoor entry in Utilities.FindAllEntitiesByDesignerName<CBaseDoor>("prop_door_rotating"))
            {
                _doorPositions.Add(entry, entry.AbsOrigin!);
            }
        }
    }
}

