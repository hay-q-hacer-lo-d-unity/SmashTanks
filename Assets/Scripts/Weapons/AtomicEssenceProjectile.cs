using System.Collections.Generic;
using Manager;
using Tank;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AtomicEssenceProjectile : Projectile
    {
        [Header("Animation Settings")] 
        public Sprite[] animationFrames;
        public float frameRate = 10f; // Frames per second
        [SerializeField] private SpriteRenderer spriteRenderer;
        private int _currentFrame;
        private float _frameTimer;


        private void Awake()
        {
            // Must be trigger to pass through objects
            projectileCollider.isTrigger = true;
        }

        private void Update()
        {
            if (rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            AnimateSprite();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore owner
            if (other == OwnerCollider) return;

            var tank = other.GetComponent<TankScript>();
            if (!tank) return;

            ApplyDamage(tank);
        }

        private void ApplyDamage(TankScript tank)
        {
            GameManagerScript.Instance.ApplyDamage(tank.OwnerId, Damage);

            SoundsScript.Play2DSound(
                SoundsScript.Instance.atomicHit,
                0.7f
            );
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
    }
}