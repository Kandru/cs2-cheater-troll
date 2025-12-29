using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;

namespace CheaterTroll.Plugins
{
    public class CrouchJump : PluginBlueprint
    {
        public override string Description => "Crouch Jump";
        public override string ClassName => "CrouchJump";
        public override List<string> Listeners =>
        [
            "OnPlayerButtonsChanged"
        ];

        private readonly Dictionary<CCSPlayerController, bool> _playerWasForcedBackwards = [];

        public CrouchJump(PluginConfig GlobalConfig, IStringLocalizer Localizer, bool IshotReloaded) : base(GlobalConfig, Localizer, IshotReloaded)
        {
            Console.WriteLine(_localizer["plugins.class.initialize"].Value.Replace("{name}", ClassName));
        }

        public override void Add(CCSPlayerController player, CheaterConfig config)
        {
            // check if player is valid and has a pawn
            if (player == null
                || !player.IsValid
                || player.Pawn?.Value == null
                || !player.Pawn.Value.IsValid)
            {
                return;
            }
            _players.Add(player, config);
            _playerWasForcedBackwards.Add(player, false);
        }

        public override void Remove(CCSPlayerController player)
        {
            _ = _players.Remove(player);
            _ = _playerWasForcedBackwards.Remove(player);
        }

        public override void Reset()
        {
            _players.Clear();
            _playerWasForcedBackwards.Clear();
        }

        public void OnPlayerButtonsChanged(CCSPlayerController player, PlayerButtons pressed, PlayerButtons released)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.IsHLTV
                || !_players.ContainsKey(player)
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.AbsOrigin == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE
                || player.PlayerPawn.Value.GroundEntity.IsValid)
            {
                return;
            }
            // only proceed if player pressed both Duck and Jump and was not already forced forward
            if (!((player.Buttons & PlayerButtons.Duck) != 0
                && (player.Buttons & PlayerButtons.Jump) != 0
                && (player.Buttons & PlayerButtons.Forward) != 0)
                || _playerWasForcedBackwards[player])
            {
                return;
            }
            // set to true to avoid double actions until we reset it
            _playerWasForcedBackwards[player] = true;
            // check if player is on ground again
            CheckIfPlayerIsOnGroundAgain(player);
            // set low impulse backwards
            Vector impulseDirection = AngleToForwardWithInvertedPitch(player.PlayerPawn.Value.V_angle) * -1f;
            Vector currentVelocity = new(player.PlayerPawn.Value.Velocity.X, player.PlayerPawn.Value.Velocity.Y, player.PlayerPawn.Value.Velocity.Z);

            Vector impulse = new(
                impulseDirection.X * 150f,
                impulseDirection.Y * 150f,
                impulseDirection.Z * 150f
            );

            Vector newVelocity = new(
                currentVelocity.X + impulse.X,
                currentVelocity.Y + impulse.Y,
                currentVelocity.Z
            );
            player.PlayerPawn.Value.Teleport(null, null, newVelocity);
        }

        public void CheckIfPlayerIsOnGroundAgain(CCSPlayerController? player)
        {
            _ = new CounterStrikeSharp.API.Modules.Timers.Timer(0.1f, () =>
            {
                if (player == null
                    || !player.IsValid
                    || !_players.ContainsKey(player)
                    || !_playerWasForcedBackwards.ContainsKey(player)
                    || player.PlayerPawn == null
                    || !player.PlayerPawn.IsValid
                    || player.PlayerPawn.Value == null)
                {
                    return;
                }
                // if player is not on ground again, recheck
                if (!player.PlayerPawn.Value.GroundEntity.IsValid)
                {
                    CheckIfPlayerIsOnGroundAgain(player);
                    return;
                }
                _playerWasForcedBackwards[player] = false;
            });
        }

        private Vector AngleToForwardWithInvertedPitch(QAngle angles)
        {
            double yawRad = angles.Y * Math.PI / 180.0;

            float x = (float)Math.Cos(yawRad);
            float y = (float)Math.Sin(yawRad);
            float z = 0;

            return new Vector(x, y, z);
        }
    }
}
