using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class HeavyKnife : PluginBlueprint
    {
        public override string Description => "Heavy Knife";
        public override string ClassName => "HeavyKnife";
        public override List<string> Events => [
            "EventItemEquip",
        ];

        public HeavyKnife(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public HookResult EventItemEquip(EventItemEquip @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || !@event.Item.Contains("knife", StringComparison.CurrentCultureIgnoreCase)
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                return HookResult.Continue;
            }
            SetPlayerSpeed(player);
            return HookResult.Continue;
        }

        private void SetPlayerSpeed(CCSPlayerController? player, float speed = -1f)
        {
            if (player?.IsValid != true)
            {
                return;
            }

            // Get the speed value before the delayed execution
            float speedToApply = speed >= 0 ? speed : _players[player].HeavyKnife.Speed;

            // delay 3 frames to ensure the velocity modifier is set correctly
            Server.NextFrame(() =>
            {
                Server.NextFrame(() =>
                {
                    Server.NextFrame(() =>
                    {
                        // Re-validate player and check if still in our tracking
                        if (player == null
                            || player?.IsValid == false
                            || player?.PlayerPawn?.IsValid == false
                            || player?.PlayerPawn?.Value == null)
                        {
                            return;
                        }
                        player.PlayerPawn.Value.VelocityModifier = speedToApply;
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawn", "m_flVelocityModifier");
                    });
                });
            });
        }
    }
}
