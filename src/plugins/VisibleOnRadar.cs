using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class VisibleOnRadar : PluginBlueprint
    {
        public override string Description => "Visible On Radar";
        public override string ClassName => "VisibleOnRadar";
        public override List<string> Listeners => [
            "OnTick"
        ];

        public VisibleOnRadar(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public void OnTick()
        {
            if (_players.Count == 0)
            {
                return;
            }

            foreach (CCSPlayerController entry in _players.Keys.ToList())
            {
                CCSPlayerPawn? playerPawn = entry.PlayerPawn?.Value;
                if (playerPawn == null
                    || !playerPawn.IsValid)
                {
                    continue;
                }
                playerPawn.EntitySpottedState.Spotted = true;
                Span<uint> spottedByMask = playerPawn.EntitySpottedState.SpottedByMask;
                for (int i = 0; i < spottedByMask.Length; i++)
                {
                    spottedByMask[i] = 1;
                }
                Utilities.SetStateChanged(playerPawn, "CCSPlayerPawn", "m_entitySpottedState", Schema.GetSchemaOffset("EntitySpottedState_t", "m_bSpotted"));
                Utilities.SetStateChanged(playerPawn, "CCSPlayerPawn", "m_entitySpottedState", Schema.GetSchemaOffset("EntitySpottedState_t", "m_bSpottedByMask"));
            }
        }
    }
}
