using Tank;
using UnityEngine;

namespace Weapons
{
    public class BouncyMissile : ExplosiveProjectile
    {
        [SerializeField] private float fuseTime = SmashTanksConstants.BOUNCY_MISSILE_FUSE_TIME;
        [Header("Animation Settings")] public Sprite[] animationFrames;

        private SpriteRenderer _spriteRenderer;
        private float _timer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Invoke(nameof(Explode), fuseTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.TryGetComponent<TankScript>(out _)) Explode();
        }

        private void Update() => AnimateSprite();

        private void AnimateSprite()
        {
            if (animationFrames == null || animationFrames.Length < 2 || _spriteRenderer == null)
                return;

            _timer += Time.deltaTime;
            var cycleTime = _timer % 1f; // 1 second total cycle

            // Show first sprite for 0.75s, second for 0.25s
            _spriteRenderer.sprite = cycleTime < 0.75f ? animationFrames[0] : animationFrames[1];
        }
    }

}