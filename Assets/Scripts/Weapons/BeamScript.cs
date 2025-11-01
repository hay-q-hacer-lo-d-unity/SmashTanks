using System.Collections.Generic;
using Tank;
using UnityEngine;

namespace Weapons
{

    public class BeamScript : MonoBehaviour
    {
        [Header("Beam Settings")]
        public float width = 0.1f;              // Grosor visual del rayo
        public float damage = 1f;               // Daño que inflige
        public float maxDistance = 50f;         // Alcance máximo del rayo
        public GameObject beamEffectPrefab;     // Prefab visual del rayo

        private LineRenderer _lineRenderer;

        void Start()
        {
            // Crear un LineRenderer si no está
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.startWidth = width;
                _lineRenderer.endWidth = width;
                _lineRenderer.positionCount = 2;
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _lineRenderer.startColor = Color.cyan;
                _lineRenderer.endColor = Color.cyan;
            }

            FireBeam();
        }

        void FireBeam()
        {
            Vector2 origin = transform.position;
            Vector2 direction = transform.up; // Asumimos que el "up" del tanque es hacia adelante

            // Detectar todos los objetos que atraviesa el rayo
            var hits = Physics2D.RaycastAll(origin, direction, maxDistance);

            var points = new List<Vector3> { origin };

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;
                points.Add(hit.point);

                // Aplicar daño si hay un tanque
                var tank = hit.collider.GetComponent<TankScript>();
                if (tank != null)
                {
                    tank.ApplyDamage(damage);
                }
            }

            // Si no choca con nada, el rayo llega hasta el maxDistance
            if (points.Count == 1)
            {
                points.Add(origin + direction * maxDistance);
            }

            _lineRenderer.SetPositions(points.ToArray());

            // Instanciar efecto visual si se desea
            if (beamEffectPrefab != null)
            {
                Instantiate(beamEffectPrefab, origin, Quaternion.identity);
            }

            // Destruir el rayo después de un breve tiempo para efecto visual
            Destroy(gameObject, 0.1f);
        }
        
        public void SetStats(float newDamage)
        {
            damage = newDamage;
        }
    }
}