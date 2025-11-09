using System.Collections;
using Tank;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Represents a basic missile projectile that explodes on impact,
    /// dealing damage and applying explosion force to nearby objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BasicMissile : ExplosiveProjectile
    {
        [Header("Animation Settings")] public Sprite[] animationFrames;
        public float frameRate = 10f; // Frames per second
        private SpriteRenderer _spriteRenderer;
        private int _currentFrame;
        private float _frameTimer;

        private void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();
        
        
        
        #region Unity Callbacks
        
        private void Update()
        {
            if (Rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(Rb.linearVelocity.y, Rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            AnimateSprite();
        }
        
        private void OnCollisionEnter2D() => Explode();

        private void AnimateSprite()
        {
            if (animationFrames == null || animationFrames.Length == 0) return;

            _frameTimer += Time.deltaTime;
            if (!(_frameTimer >= 1f / frameRate)) return;
            _frameTimer = 0f;
            _currentFrame = (_currentFrame + 1) % animationFrames.Length;
            _spriteRenderer.sprite = animationFrames[_currentFrame];
        }

        #endregion
    }
}
