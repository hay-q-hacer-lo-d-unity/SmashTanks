using UnityEngine;

namespace Tank
{
    public class TrajectoryDrawerScript : MonoBehaviour
    {
        public int points = 50;
        public float gravity = SmashTanksConstants.GRAVITY;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = points;
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = Color.white;
            _lineRenderer.widthMultiplier = 0.05f; //grosor
        }

        public void DrawParabola(Vector2 origin, Vector2 initialVelocity, float accuracy)
        {
            if (!_lineRenderer) return;

            _lineRenderer.positionCount = points; // reset position count

            for (var i = 0; i < points; i++)
            {
                var t = accuracy * i / (points - 1);
                var x = origin.x + initialVelocity.x * t;
                var y = origin.y + initialVelocity.y * t + 0.5f * gravity * t * t;
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }   
        }
    
        public void ClearParabola()
        {
            if (!_lineRenderer) return;
            _lineRenderer.positionCount = 0;
        }
    }
}
