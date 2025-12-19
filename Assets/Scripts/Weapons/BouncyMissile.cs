using Actions;
using Manager;
using Tank;
using UnityEngine;

namespace Weapons
{
    public class BouncyMissile : ExplosiveProjectile
    {
        [SerializeField] private float fuseTime = SmashTanksConstants.BouncyMissile.FuseTime;
        [Header("Animation Settings")] public Sprite[] animationFrames;

        [SerializeField] private SpriteRenderer spriteRenderer;
        private float _timer;

        private void Start()
        {
            Invoke(nameof(Explode), fuseTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.TryGetComponent<TankScript>(out _)) return;
            SoundsScript.Play2DSound(SoundsScript.Instance.crowdCheerHit, 0.8f);
            Explode();
        }

        private void Update() => AnimateSprite();

        private void AnimateSprite()
        {
            if (animationFrames == null || animationFrames.Length < 2 || spriteRenderer == null)
                return;

            _timer += Time.deltaTime;
            var cycleTime = _timer % 1f; // 1 second total cycle

            // Show first sprite for 0.75s, second for 0.25s
            spriteRenderer.sprite = cycleTime < 0.75f ? animationFrames[0] : animationFrames[1];
        }
    }

}