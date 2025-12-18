using Tank;
using UnityEngine;

namespace Actions
{
    public sealed class ActionContext
    {
        public TankScript Tank { get; }
        public TankStats Stats { get; }

        public Rigidbody2D Rb { get; }
        public Collider2D Collider { get; }
        public Transform FirePoint { get; }
        public Transform Center { get; }

        // Prefabs
        public GameObject MissilePrefab { get; }
        public GameObject BouncyMissilePrefab { get; }
        public GameObject BeamPrefab { get; }
        public GameObject GalePrefab { get; }
        public GameObject JuggernautProjectilePrefab { get; }

        // 🔊 Audio
        public AudioClip ExecuteSound { get; }

        public ActionContext(TankScript tank, AudioClip executeSound)
        {
            Tank = tank;
            Stats = tank.Stats;

            Rb = tank.Rb;
            Collider = tank.Collider;
            FirePoint = tank.FirePoint;
            Center = tank.Center;

            MissilePrefab = tank.MissilePrefab;
            BouncyMissilePrefab = tank.BouncyMissilePrefab;
            BeamPrefab = tank.BeamPrefab;
            GalePrefab = tank.GalePrefab;
            JuggernautProjectilePrefab = tank.JuggernautProjectilePrefab;

            ExecuteSound = executeSound;
        }
    }
}
