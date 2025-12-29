using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Drawing;

namespace CheaterTroll.Utils
{
    public static class Glow
    {
        public static (CDynamicProp?, CDynamicProp?) Create(CBaseEntity entity, Color color, int team = -1, string type = "prop_dynamic")
        {
            CDynamicProp? _glowProxy = Utilities.CreateEntityByName<CDynamicProp>(type);
            CDynamicProp? _glow = Utilities.CreateEntityByName<CDynamicProp>(type);
            if (_glowProxy == null
                || _glow == null)
            {
                return (null, null);
            }

            string modelName = entity.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;
            // create proxy
            _glowProxy.Spawnflags = 256u;
            _glowProxy.RenderMode = RenderMode_t.kRenderNone;
            _glowProxy.DispatchSpawn();
            _glowProxy.AcceptInput("FollowEntity", entity, _glowProxy, "!activator");
            _glowProxy.SetModel(modelName);
            // create glow
            _glow.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);
            _glow.DispatchSpawn();
            _glow.SetModel(modelName);
            _glow.AcceptInput("FollowEntity", _glowProxy, _glow, "!activator");
            _glow.Spawnflags = 256u;
            _glow.Render = Color.FromArgb(255, 255, 255, 255);
            _glow.Glow.GlowColorOverride = color;
            _glow.RenderMode = RenderMode_t.kRenderGlow;
            _glow.Glow.GlowRange = 5000;
            _glow.Glow.GlowTeam = -1;
            _glow.Glow.GlowType = 3;
            _glow.Glow.GlowRangeMin = 50;
            _glow.Glow.GlowTeam = team;
            return (_glowProxy, _glow);
        }

        public static void RemoveGlow(CBaseEntity? glowProxy, CBaseEntity? glow)
        {
            Entities.RemoveEntity(glowProxy);
            Entities.RemoveEntity(glow);
        }
    }
}