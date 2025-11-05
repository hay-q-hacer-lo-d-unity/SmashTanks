using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tank;
using UnityEngine;

namespace Weapons
{
    public class BeamScript : MonoBehaviour
    {
        [Header("Beam Settings")]
        public float width = 2f;                  // Grosor visual del rayo
        public float damage = 1f;                 // Daño base
        public float maxDistance = 50f;           // Alcance máximo
        public float fadeDuration = 1f;         // Duración del fade-out
        public GameObject beamEffectPrefab;       // Efecto visual opcional

        private LineRenderer _lineRenderer;

        void Start()
        {
            // Crear o configurar el LineRenderer
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
                _lineRenderer = gameObject.AddComponent<LineRenderer>();

            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _lineRenderer.positionCount = 2;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = Color.cyan;
            _lineRenderer.endColor = Color.cyan;

            FireBeam();
        }

        void FireBeam()
        {
            Vector2 origin = transform.position;
            Vector2 direction = transform.up;

            // Detectar todos los impactos
            var hits = Physics2D.RaycastAll(origin, direction, maxDistance);
            var sortedHits = hits.OrderBy(h => h.distance).ToArray();

            var currentDamage = damage;

            foreach (var hit in sortedHits)
            {
                if (hit.collider == null) continue;

                var tank = hit.collider.GetComponent<TankScript>();
                if (tank == null) continue;
                tank.ApplyDamage(currentDamage);
                currentDamage *= 2f; // ⚡ Duplicar daño por cada tanque atravesado
            }

            // Dibujar el rayo hasta el máximo alcance
            var points = new List<Vector3> { origin, origin + direction * maxDistance };
            _lineRenderer.SetPositions(points.ToArray());

            // Instanciar efecto opcional
            if (beamEffectPrefab != null)
                Instantiate(beamEffectPrefab, origin, Quaternion.identity);

            // Iniciar desvanecimiento
            StartCoroutine(FadeOutAndDestroy());
        }

        private IEnumerator FadeOutAndDestroy()
        {
            var elapsed = 0f;
            var startColor = _lineRenderer.startColor;
            var endColor = startColor;
            endColor.a = 0f; // Transparente

            var startWidth = _lineRenderer.startWidth;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / fadeDuration;

                // Interpolar color y grosor
                var currentColor = Color.Lerp(startColor, endColor, t);
                _lineRenderer.startColor = currentColor;
                _lineRenderer.endColor = currentColor;
                _lineRenderer.startWidth = Mathf.Lerp(startWidth, 0f, t);
                _lineRenderer.endWidth = Mathf.Lerp(startWidth, 0f, t);

                yield return null;
            }

            Destroy(gameObject);
        }

        public void SetStats(float newDamage)
        {
            damage = newDamage;
        }
    }
}
