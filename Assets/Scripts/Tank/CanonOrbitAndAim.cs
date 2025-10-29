using UnityEngine;

namespace Tank
{
    public class CanonOrbitAndAim : MonoBehaviour
    {
        [SerializeField] Transform tank;         // arrastrá tu Tank
        [SerializeField] Transform canonSprite;  // arrastrá el objeto "Canon"
        [SerializeField] float radius = 0.8f;    // ajustá hasta que “toque” el borde
        [SerializeField] float spriteAngleOffset = 0f; // ej: -90 si tu sprite mira “arriba”
        [SerializeField] public bool canMove = true;

        private bool isLocked = false;
        private Vector3 relativeOffset;  // posición fija relativa al tanque
        private Quaternion fixedRotation;

        void Start()
        {
            // Guardamos la posición inicial del cañón relativa al tanque
            if (tank != null)
            {
                relativeOffset = transform.position - tank.position;
            }
        }

        void Update()
        {
            if (!tank) return;

            if (isLocked)
            {
                // Mantiene posición fija relativa al tanque
                transform.position = tank.position + relativeOffset;
                canonSprite.rotation = fixedRotation;
                return;
            }

            if (!canMove) return;

            // Mouse en mundo (2D)
            var m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m.z = tank.position.z;

            // Dirección desde el centro del Tank
            Vector3 dir = (m - tank.position).normalized;

            // 1) Deslizar el pivot por la superficie (círculo)
            transform.position = tank.position + dir * radius;

            // 2) Rotar el sprite para apuntar al mouse
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteAngleOffset;
            canonSprite.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void LockCannonPosition()
        {
            if (!tank) return;

            isLocked = true;
            relativeOffset = transform.position - tank.position;
            fixedRotation = tank.transform.rotation;
        }

        public void UnlockCannonPosition()
        {
            isLocked = false;
        }
    }
}
