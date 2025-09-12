using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TrajectoryDrawerScript : MonoBehaviour
    {
        public int points = 50;
        public float gravity = -9.81f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = points;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.widthMultiplier = 0.05f; //grosor
        }

        public void DrawParabola(Vector2 origin, Vector2 initialVelocity)
        {
            float tMax = CalculateTimeToHitGround(origin.y, initialVelocity.y, gravity);

            for (int i = 0; i < points; i++)
            {
                float t = tMax * i / (points - 1); // dividimos el tiempo en "points" pasos
                float x = origin.x + initialVelocity.x * t;
                float y = origin.y + initialVelocity.y * t + 0.5f * gravity * t * t;
                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }   
        }
        
        private float CalculateTimeToHitGround(float y0, float vy, float g)
        {
            // Resolviendo y = y0 + vy*t + 0.5*g*t^2 = 0
            float discriminant = vy * vy - 2 * g * y0;
            if (discriminant < 0) discriminant = 0; // seguridad
            return (-vy - Mathf.Sqrt(discriminant)) / g;
        }
    }
}