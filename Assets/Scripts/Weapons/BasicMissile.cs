using System.Collections;
using Actions;
using Manager;
using Tank;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Header("Animation Settings")] 
        public Sprite[] animationFrames;
        public float frameRate = 10f; // Frames per second
        [SerializeField] private SpriteRenderer spriteRenderer;
        private int _currentFrame;
        private float _frameTimer;

        
        
        #region Unity Callbacks
        
        private void Update()
        {
            if (rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            AnimateSprite();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<TankScript>())
            {
                SoundsScript.Play2DSound(SoundsScript.Instance.crowdCheerHit, 0.8f);
            }
            Explode();
        }

        private void AnimateSprite()
        {
            if (animationFrames == null || animationFrames.Length == 0) return;

            _frameTimer += Time.deltaTime;
            if (!(_frameTimer >= 1f / frameRate)) return;
            _frameTimer = 0f;
            _currentFrame = (_currentFrame + 1) % animationFrames.Length;
            spriteRenderer.sprite = animationFrames[_currentFrame];
        }

        #endregion
    }
}
