using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _AlwaysDoorClosedEnabled = false;
        private List<long> _AlwaysDoorClosedDoorsInUse = [];

        private void InitializeAlwaysDoorClosed()
        {
            // skip if already enabled
            if (_AlwaysDoorClosedEnabled) return;
            // register listener
            RegisterEventHandler<EventDoorOpen>(OnDoorOpen);
            _AlwaysDoorClosedEnabled = true;
            DebugPrint("Plugin AlwaysDoorClosed enabled");
        }

        private void ResetAlwaysDoorClosed()
        {
            // remove listener
            DeregisterEventHandler<EventDoorOpen>(OnDoorOpen);
            // disable plug-in
            _AlwaysDoorClosedEnabled = false;
            _AlwaysDoorClosedDoorsInUse.Clear();
            DebugPrint("Plugin AlwaysDoorClosed disabled");
        }

        private HookResult OnDoorOpen(EventDoorOpen @event, GameEventInfo info)
        {
            DebugPrint("OnDoorOpen");
            long doorIndex = @event.Entindex;
            // if there is already one action on this door, skip
            if (_AlwaysDoorClosedDoorsInUse.Contains(doorIndex)) return HookResult.Continue;
            // get player who opened the door
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || string.IsNullOrEmpty(player.NetworkIDString)
                || player.IsBot
                || player.IsHLTV
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) return HookResult.Continue;
            // check if player is a cheater
            if (_onlineCheaters.TryGetValue(player.NetworkIDString, out CheaterConfig? cheaterConfig))
            {
                CRotDoor? door = Utilities.GetEntityFromIndex<CRotDoor>((int)doorIndex);
                if (door == null
                    || !door.IsValid) return HookResult.Continue;
                if (new Random().Next(0, 2) == 0)
                {
                    Console.WriteLine("open door slowly a little bit");
                    _AlwaysDoorClosedDoorsInUse.Add(doorIndex);
                    float oldSpeed = door.Speed;
                    door.AcceptInput("SetSpeed", door, door, Config.AlwaysDoorClosed.Speed.ToString());
                    AddTimer(Config.AlwaysDoorClosed.Delay, () =>
                    {
                        // reset door
                        _AlwaysDoorClosedDoorsInUse.Remove(doorIndex);
                        if (door == null || !door.IsValid) return;
                        // reset speed
                        door.AcceptInput("SetSpeed", door, door, oldSpeed.ToString());
                        // close again
                        door.AcceptInput("Close");
                    });
                    return HookResult.Continue;
                }
                else
                {
                    Console.WriteLine("close door instantly and knock :D");
                    door.AcceptInput("Close");
                    door.EmitSound(Config.AlwaysDoorClosed.Sound);
                }
            }
            return HookResult.Continue;
        }
    }
}
