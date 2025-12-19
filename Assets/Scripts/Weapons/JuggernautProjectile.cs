using Tank;
using UnityEngine;

namespace Weapons
{
    public class JuggernautProjectile : Projectile
    {
        [Header("Animation Settings")] 
        public Sprite[] animationFrames;
        public float frameRate = 10f; // Frames per second
        [SerializeField] private SpriteRenderer spriteRenderer;
        private int _currentFrame;
        private float _frameTimer;
        private void Update()
        {
            if (rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            AnimateSprite();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var tank = collision.gameObject.GetComponent<TankScript>();
            if (tank != null) tank.ApplyDamage(Damage);
            Debug.Log($"Projectile hit {collision.collider.name}");
            Destroy(gameObject);
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