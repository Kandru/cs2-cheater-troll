using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CheaterTroll.Enums;
using CheaterTroll.Utils;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Entities;
using System.IO.Compression;

namespace CheaterTroll.Plugins
{
    public class DamageControl : PluginBlueprint
    {
        public override string Description => "Damage Control";
        public override string ClassName => "DamageControl";
        public override List<string> Listeners => [
            "CheckTransmit",
            "OnEntityTakeDamagePre"
        ];

        public DamageControl(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public HookResult OnEntityTakeDamagePre(CBaseEntity entity, CTakeDamageInfo info)
        {
            // only check when a player got hit
            if (entity.DesignerName != "player"
                || info.Attacker == null
                || !info.Attacker.IsValid
                || info.Attacker.Value == null
                || !info.Attacker.Value.IsValid)
            {
                return HookResult.Continue;
            }
            // get attacker and victim
            CCSPlayerPawn? attackerPawn = info.Attacker.Value.As<CCSPlayerPawn>();
            CCSPlayerPawn? victimPawn = entity.As<CCSPlayerPawn>();
            if (attackerPawn == null
                || !attackerPawn.IsValid
                || attackerPawn.Controller == null
                || !attackerPawn.Controller.IsValid
                || attackerPawn.Controller.Value == null
                || !attackerPawn.Controller.Value.IsValid
                || victimPawn == null
                || !victimPawn.IsValid)
            {
                return HookResult.Continue;
            }
            // check if attacker is a cheater
            var attackerController = attackerPawn.Controller.Value.As<CCSPlayerController>();
            if (!_players.TryGetValue(attackerController, out var playerData))
            {
                return HookResult.Continue;
            }

            // check if we should deal self-damage
            if (playerData.DamageControl.SelfDamage
                && attackerPawn.TeamNum != victimPawn.TeamNum)
            {
                attackerPawn.Health -= (int)Math.Max(-1, Math.Round(info.Damage * playerData.DamageControl.SelfDamagePercentage));
                if (attackerPawn.Health <= 0)
                {
                    // delay kill a frame to allow OnTakeDamage to finish
                    Server.NextFrame(() => attackerController.CommitSuicide(false, true));
                }
                Utilities.SetStateChanged(attackerPawn, "CBaseEntity", "m_iHealth");
            }

            // check if damage limit is enabled and limit it
            if (playerData.DamageControl.DamageLimit)
            {
                info.Damage = Math.Min(info.Damage, playerData.DamageControl.MaxDamage);
            }
            return HookResult.Continue;
        }
    }
}
