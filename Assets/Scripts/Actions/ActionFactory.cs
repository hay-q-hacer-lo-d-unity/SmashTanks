using Tank;
using UnityEngine;

namespace Actions
{
    public static class ActionFactory
    {
        public static IAction Create(string actionId, TankScript tank)
        {
            if (!tank) return null;

            return actionId.ToLowerInvariant() switch
            {
                "action_missile"        => new Missile       (CreateCtx(tank, Sounds.Instance.missile)),
                "action_bouncy_missile" => new BouncyMissile (CreateCtx(tank, Sounds.Instance.bouncyMissile)),
                "action_jump"           => new Jump          (CreateCtx(tank, Sounds.Instance.jump)),
                "action_crash"          => new Crash         (CreateCtx(tank, Sounds.Instance.crash)),
                "action_beam"           => new Beam          (CreateCtx(tank, Sounds.Instance.beam)),
                "action_teleport"       => new Teleport      (CreateCtx(tank, Sounds.Instance.teleport)),
                "action_gale"           => new Gale          (CreateCtx(tank, Sounds.Instance.gale)),
                "juggernaut"            => new JuggernautUlti(CreateCtx(tank, Sounds.Instance.juggernaut)),
                _                       => null
            };
        }

        private static ActionContext CreateCtx(TankScript tank, AudioClip sound) => new(tank, sound);
    }
}
