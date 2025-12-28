using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class GrenadeSelfDamage : PluginBlueprint
    {
        public override string Description => "Grenade Self Damage";
        public override string ClassName => "GrenadeSelfDamage";
        public override List<string> Events => [
            "EventHegrenadeDetonate",
            "EventMolotovDetonate",
            "EventFlashbangDetonate"
        ];

        public GrenadeSelfDamage(PluginConfig GlobalConfig, IStringLocalizer Localizer) : base(GlobalConfig, Localizer)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public HookResult EventHegrenadeDetonate(EventHegrenadeDetonate @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !_players[player].GrenadeSelfDamage.Enabled
                || !_players[player].GrenadeSelfDamage.EnableHEGrenades
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }

            CHEGrenadeProjectile? grenade = Utilities.GetEntityFromIndex<CHEGrenadeProjectile>(@event.Entityid);
            if (grenade == null || !grenade.IsValid)
            {
                return HookResult.Continue;
            }

            grenade.Teleport(player.PlayerPawn.Value.AbsOrigin);
            return HookResult.Continue;
        }

        public HookResult EventMolotovDetonate(EventMolotovDetonate @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !_players[player].GrenadeSelfDamage.Enabled
                || !_players[player].GrenadeSelfDamage.EnableMolotovs
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }
            // to avoid further damage to other players teleport player to grenade
            player.PlayerPawn.Value.Teleport(new Vector(@event.X, @event.Y, @event.Z));
            // give player only one hp left to die instantl
            player.PlayerPawn.Value.Health = 1;
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
            return HookResult.Continue;
        }

        public HookResult EventFlashbangDetonate(EventFlashbangDetonate @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !_players[player].GrenadeSelfDamage.Enabled
                || !_players[player].GrenadeSelfDamage.EnableFlashBangs
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }
            // simply full-blind the player
            player.PlayerPawn.Value.FlashDuration = _globalConfig.Plugins.GrenadeSelfDamage.FlashBangDuration;
            player.PlayerPawn.Value.FlashMaxAlpha = 255;
            player.PlayerPawn.Value.BlindStartTime = Server.CurrentTime;
            player.PlayerPawn.Value.BlindUntilTime = Server.CurrentTime + _globalConfig.Plugins.GrenadeSelfDamage.FlashBangDuration;
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawnBase", "m_flFlashMaxAlpha");
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawnBase", "m_flFlashDuration");
            return HookResult.Continue;
        }
    }
}
