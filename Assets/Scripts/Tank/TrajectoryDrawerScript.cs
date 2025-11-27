using UnityEngine;

namespace Tank
{
    public class TrajectoryDrawerScript : MonoBehaviour
    {
        public int segments = 50;
        public float gravity = SmashTanksConstants.Physics.Gravity;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = segments;
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = Color.white;
            _lineRenderer.widthMultiplier = 0.05f; //grosor
        }

        public void DrawParabola_ByTime(Vector2 origin, Vector2 initialVelocity, float accuracy)
        {
            if (!_lineRenderer) return;

            _lineRenderer.positionCount = segments;

            for (var i = 0; i < segments; i++)
            {
                var t = accuracy * i / (segments - 1);
                var x = origin.x + initialVelocity.x * t;
                var y = origin.y + initialVelocity.y * t + 0.5f * gravity * t * t;
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }   
        }
        
        public void DrawParabola_ByDistanceBetweenPoints(Vector2 origin, Vector2 initialVelocity, float accuracy)
        {
            if (!_lineRenderer) return;

            _lineRenderer.positionCount = segments;

            var vx = initialVelocity.x;
            var vy = initialVelocity.y;

            var t = 0f;
            const float dt = 0.02f;
            var endTime = 0f;

            while (true)
            {
                var x = origin.x + vx * t;
                var y = origin.y + vy * t + 0.5f * gravity * t * t;

                var dist = Vector2.Distance(origin, new Vector2(x, y));

                if (dist >= accuracy)
                {
                    endTime = t;
                    break;
                }

                t += dt;

                if (t > 100f) break;
            }
            
            for (var i = 0; i < segments; i++)
            {
                var ti = endTime * i / (segments - 1);
                var x = origin.x + vx * ti;
                var y = origin.y + vy * ti + 0.5f * gravity * ti * ti;

                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }

        public void DrawParabola_ByArcLength(Vector2 origin, Vector2 initialVelocity, float accuracy)
        {
            if (!_lineRenderer) return;

            _lineRenderer.positionCount = segments;

            var vx = initialVelocity.x;
            var vy = initialVelocity.y;

            var g = gravity;
            
            var low = 0f;
            var high = 5f;

            const float epsilon = 0.001f;

            while (high - low > epsilon)
            {
                var mid = (low + high) * 0.5f;
                var length = ArcLength(mid, vx, vy, g);

                if (length < accuracy)
                    low = mid;
                else
                    high = mid;
            }

            var endTime = (low + high) * 0.5f;

            for (var i = 0; i < segments; i++)
            {
                var t = endTime * i / (segments - 1);
                var x = origin.x + vx * t;
                var y = origin.y + vy * t + 0.5f * g * t * t;

                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }

        
        private static float ArcLength(float T, float vx, float vy, float g)
        {
            const int n = 50;
            var h = T / n;

            var sum = 0f;

            for (var i = 0; i <= n; i++)
            {
                var t = i * h;

                var dy = vy + g * t;

                var v = Mathf.Sqrt(vx * vx + dy * dy);

                var w = i is 0 or n ? 1 : (i % 2 == 0 ? 2 : 4);
                sum += w * v;
            }

            return sum * (h / 3f);
        }


    
        public void ClearParabola()
        {
            if (!_lineRenderer) return;
            _lineRenderer.positionCount = 0;
        }

        public void DrawHalfLine(Vector2 start, Vector2 end, float length = 1000f)
        {
            if (!_lineRenderer) return;

            // Calculate direction
            var direction = (end - start).normalized;

            // Extend line in that direction for a long distance
            var extendedEnd = start + direction * length;

            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, new Vector3(start.x, start.y, 0));
            _lineRenderer.SetPosition(1, new Vector3(extendedEnd.x, extendedEnd.y, 0));
        }
        
        public void DrawSegment(Vector2 start, Vector2 end)
        {
            if (!_lineRenderer) return;

            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, new Vector3(start.x, start.y, 0));
            _lineRenderer.SetPosition(1, new Vector3(end.x, end.y, 0));
        }

        public void DrawCircle(Vector2 center, float radius)
        {
            var angleStep = 2 * Mathf.PI / segments;
            var points = new Vector3[segments];

            for (var i = 0; i < segments; i++)
            {
                var angle = i * angleStep;
                var x = center.x + Mathf.Cos(angle) * radius;
                var y = center.y + Mathf.Sin(angle) * radius;
                
                points[i] = new Vector3(x, y, 0f);
            }
            points[segments - 1] = points[0]; // Close the loop

            _lineRenderer.positionCount = points.Length;
            _lineRenderer.SetPositions(points);
        }
        
        public void DrawGaleZone(Vector2 origin, Vector2 target)
        {
            if (!_lineRenderer) return;

            // Compute main direction
            var dir = (target - origin).normalized;
            var angleForward = Mathf.Atan2(dir.y, dir.x);

            // Prepare vertex array
            var totalPoints = segments * 2; // one semicircle per point set
            var points = new Vector3[totalPoints];
            const float radius = SmashTanksConstants.Gale.Radius;

            // --- Draw semicircle at origin ---
            for (var i = 0; i < segments; i++)
            {
                // Angle runs from -90° to +90° relative to direction
                var angle = angleForward + Mathf.Deg2Rad * (-90f + (180f * i / (segments - 1)));
                var x = origin.x + Mathf.Cos(angle) * radius;
                var y = origin.y + Mathf.Sin(angle) * radius;
                points[i] = new Vector3(x, y, 0f);
            }

            // --- Draw semicircle at target ---
            for (var i = 0; i < segments; i++)
            {
                var angle = angleForward + Mathf.Deg2Rad * (90f - (180f * i / (segments - 1))); 
                var x = target.x + Mathf.Cos(angle) * radius;
                var y = target.y + Mathf.Sin(angle) * radius;
                points[i + segments] = new Vector3(x, y, 0f);
            }
            points[totalPoints - 1] = points[0]; // Close the loop

            // Draw them
            _lineRenderer.positionCount = points.Length;
            _lineRenderer.SetPositions(points);
        }
    }
}
