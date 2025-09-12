using UnityEngine;

namespace DefaultNamespace
{
    public class MousePointerScript : PointerScript
    {
        public override Vector2 GetDirection(Vector2 origin)
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            if (Camera.main != null)
            {
                Vector3 mouseWorld3D = Camera.main.ScreenToViewportPoint(mouseScreenPos);

                Vector2 mouseWorld = new Vector2(mouseWorld3D.x, mouseWorld3D.y);
            
                Vector2 direction = mouseWorld - origin;

                return direction.normalized;
            }
            else
            {
                return Vector2.up;
            }
        }
    }
}