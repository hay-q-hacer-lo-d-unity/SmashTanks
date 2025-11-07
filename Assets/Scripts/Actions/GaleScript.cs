using UnityEngine;

namespace Actions
{
    /// <summary>
    /// Represents a moving gust of wind that applies force to nearby rigidbodies.
    /// Travels a fixed distance before dissipating.
    /// </summary>
    public class GaleScript : MonoBehaviour
    {
        private Vector2 _direction;
        private float _force;
        private const float Radius = SmashTanksConstants.GALE_RADIUS;
        private const float Speed = SmashTanksConstants.GALE_SPEED;
        private const float FadeFraction = 0.2f;

        private float _distanceTraveled;
        private const float MaxDistance = SmashTanksConstants.GALE_DISTANCE; // The total distance this gale should travel
        private float _computedDuration; // Derived from distance / speed
        private float _timer;

        private Collider2D _ownerCollider;
        private SpriteRenderer[] _renderers;
        private Vector3 _startPosition;

        /// <summary>
        /// Initializes the gale with direction, force, and owner collider.
        /// </summary>
        public void Initialize(Vector2 direction, float force, Collider2D ownerCollider)
        {
            _direction = direction.normalized;
            _force = force;
            _ownerCollider = ownerCollider;

            _computedDuration = MaxDistance / Speed; // used only for fading logic

            // Set initial state
            _startPosition = transform.position;
            _distanceTraveled = 0f;
            _timer = 0f;

            // Visually scale gale to match radius
            const float diameter = Radius * 2f;
            transform.localScale = new Vector3(diameter, diameter, 1f);
            // Rotate sprite to match direction
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Cache renderers and start fully transparent
            _renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            SetAlpha(0f);
            
        }

        private void Update()
        {
            var delta = Speed * Time.deltaTime;
            _timer += Time.deltaTime;
            _distanceTraveled += delta;

            // Move the gale
            transform.position += (Vector3)_direction * delta;

            // Fade effects based on computed duration
            UpdateAlpha();

            // Apply forces
            ApplyWindForces();

            // Check if traveled full distance
            if (_distanceTraveled >= MaxDistance)
                Destroy(gameObject);
        }

        private void ApplyWindForces()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, Radius);
            foreach (var hit in hits)
            {
                if (!hit.attachedRigidbody) continue;
                if (hit == _ownerCollider) continue;

                Vector2 toTarget = (hit.transform.position - transform.position);
                var dot = Vector2.Dot(toTarget.normalized, _direction);

                // Only affect objects in front (same semicircle as direction)
                if (dot <= 0f) continue;

                // Apply force with falloff by distance
                var distance = toTarget.magnitude;
                var falloff = Mathf.Clamp01(1f - (distance / Radius));
                hit.attachedRigidbody.AddForce(_direction * (_force * falloff), ForceMode2D.Force);
            }
        }

        private void UpdateAlpha()
        {
            var fadeTime = _computedDuration * FadeFraction;
            var alpha = 1f;

            if (_timer < fadeTime)
            {
                // Fade in
                alpha = Mathf.Clamp01(_timer / fadeTime);
            }
            else if (_timer > _computedDuration - fadeTime)
            {
                // Fade out
                alpha = Mathf.Clamp01((_computedDuration - _timer) / fadeTime);
            }

            SetAlpha(alpha);
        }

        private void SetAlpha(float alpha)
        {
            if (_renderers == null) return;
            foreach (var r in _renderers)
            {
                if (!r) continue;
                var color = r.color;
                color.a = alpha;
                r.color = color;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
