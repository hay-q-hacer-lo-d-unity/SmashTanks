using UnityEngine;

namespace DefaultNamespace
{
    public class TrajectoryDrawerScript : MonoBehaviour
    {
        public int points = 50;
        public float gravity = -9.81f;
        public float curveLength = 2f; // Duración máxima de la curva en segundos

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = points;
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = Color.white;
            _lineRenderer.widthMultiplier = 0.05f; //grosor
        }

        public void DrawParabola(Vector2 origin, Vector2 initialVelocity)
        {
            float tMax = CalculateTimeToHitGround(origin.y, initialVelocity.y, gravity);
            for (int i = 0; i < points; i++)
            {
                float t = curveLength * i / (points - 1);
                float x = origin.x + initialVelocity.x * t;
                float y = origin.y + initialVelocity.y * t + 0.5f * gravity * t * t;
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }   
        }
        
        private float CalculateTimeToHitGround(float y0, float vy, float g)
        {
            float a = 0.5f * g;
            float b = vy;
            float c = y0;
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return -1f;
            float sqrtDisc = Mathf.Sqrt(discriminant);
            float t1 = (-b + sqrtDisc) / (2 * a);
            float t2 = (-b - sqrtDisc) / (2 * a);
            float tMax = Mathf.Max(t1, t2);
            if (tMax < 0) return -1f;
            return tMax;
        }
    }
}