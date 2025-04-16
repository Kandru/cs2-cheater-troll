using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin, IPluginConfig<PluginConfig>
    {
        private bool _GrenadeSelfDamageEnabled = false;

        private void InitializeGrenadeSelfDamage()
        {
            if (!Config.GrenadeSelfDamage.Enabled) return;
            // skip if already enabled
            if (_GrenadeSelfDamageEnabled) return;
            // register listener
            if (Config.GrenadeSelfDamage.EnableHEGrenades) RegisterEventHandler<EventHegrenadeDetonate>(OnHEGrenadeDetonate);
            if (Config.GrenadeSelfDamage.EnableMolotovs) RegisterEventHandler<EventMolotovDetonate>(OnMolotovDetonate);
            if (Config.GrenadeSelfDamage.EnableFlashBangs) RegisterEventHandler<EventFlashbangDetonate>(OnFlashBangDetonate);
            _GrenadeSelfDamageEnabled = true;
            DebugPrint("Plugin GrenadeSelfDamage enabled");
        }

        private void ResetGrenadeSelfDamage()
        {
            // remove listener
            DeregisterEventHandler<EventHegrenadeDetonate>(OnHEGrenadeDetonate);
            DeregisterEventHandler<EventMolotovDetonate>(OnMolotovDetonate);
            DeregisterEventHandler<EventFlashbangDetonate>(OnFlashBangDetonate);
            // disable plug-in
            _GrenadeSelfDamageEnabled = false;
            DebugPrint("Plugin GrenadeSelfDamage disabled");
        }

        private HookResult OnHEGrenadeDetonate(EventHegrenadeDetonate @event, GameEventInfo info)
        {
            DebugPrint("OnHEGrenadeDetonate");
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
                if (!cheaterConfig.GrenadeSelfDamage.Enabled) return HookResult.Continue;
                // teleport grenade to player :P
                CHEGrenadeProjectile? grenade = Utilities.GetEntityFromIndex<CHEGrenadeProjectile>(@event.Entityid);
                if (grenade == null || !grenade.IsValid) return HookResult.Continue;
                grenade.Teleport(player.PlayerPawn.Value.AbsOrigin);

            }
            return HookResult.Continue;
        }

        private HookResult OnMolotovDetonate(EventMolotovDetonate @event, GameEventInfo info)
        {
            DebugPrint("OnMolotovDetonate");
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
                // to avoid further damage to other players teleport player to grenade
                player.PlayerPawn.Value.Teleport(new Vector(@event.X, @event.Y, @event.Z));
                // give player only one hp left to die instantl
                player.PlayerPawn.Value.Health = 1;
            }
            return HookResult.Continue;
        }

        private HookResult OnFlashBangDetonate(EventFlashbangDetonate @event, GameEventInfo info)
        {
            DebugPrint("OnFlashBangDetonate");
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
                // simply full-blind the player
                player.PlayerPawn.Value.FlashDuration = Config.GrenadeSelfDamage.FlashBangDuration;
                player.PlayerPawn.Value.FlashMaxAlpha = 255;
                player.PlayerPawn.Value!.BlindStartTime = Server.CurrentTime;
                player.PlayerPawn.Value.BlindUntilTime = Server.CurrentTime + Config.GrenadeSelfDamage.FlashBangDuration;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawnBase", "m_flFlashDuration");
            }
            return HookResult.Continue;
        }
    }

}
