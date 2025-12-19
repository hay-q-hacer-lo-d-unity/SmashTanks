using Manager;
using Tank;
using UnityEngine;

namespace Actions
{
    public static class ActionFactory
    {
        public static IAction Create(string actionId, TankScript tank)
        {
            if (!tank) return null;
            return Utils.SnakeFromTitle(actionId) switch
            {
                "action_missile"        => new Missile          (CreateCtx(tank, SoundsScript.Instance.missile)),
                "action_bouncy_missile" => new BouncyMissile    (CreateCtx(tank, SoundsScript.Instance.bouncyMissile)),
                "action_jump"           => new Jump             (CreateCtx(tank, SoundsScript.Instance.jump)),
                "action_crash"          => new Crash            (CreateCtx(tank, SoundsScript.Instance.crash)),
                "action_beam"           => new Beam             (CreateCtx(tank, SoundsScript.Instance.beam)),
                "action_teleport"       => new Teleport         (CreateCtx(tank, SoundsScript.Instance.teleport)),
                "action_gale"           => new Gale             (CreateCtx(tank, SoundsScript.Instance.gale)),
                "juggernaut"            => new JuggernautUlti   (CreateCtx(tank, SoundsScript.Instance.juggernaut)), 
                "atomic_essence"        => new AtomicEssenceUlti(CreateCtx(tank, SoundsScript.Instance.atomicEssence)),
                _                       => null
            };
        }

        private static ActionContext CreateCtx(TankScript tank, AudioClip sound) => new(tank, sound);
    }
}
