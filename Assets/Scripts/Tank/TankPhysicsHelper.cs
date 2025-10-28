namespace Tank
{
    using UnityEngine;

    public static class TankPhysicsHelper
    {
        public static Vector2 CalculateInitialVelocity(TankScript tank, string actionName)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 cursorPos = new(mouseWorld.x, mouseWorld.y);
            Transform aim = tank.transform.Find("AimPoint");
            Transform fire = tank.transform.Find("FirePoint");
            Rigidbody2D rb = tank.GetComponent<Rigidbody2D>();

            Vector2 dir = (cursorPos - (Vector2)aim.position).normalized;
            float distance = Vector2.Distance(cursorPos, aim.position);

            switch (actionName)
            {
                case "Shoot":
                    float shootSpeed = Mathf.Clamp(distance * 5f, 0, 20f);
                    return dir * shootSpeed;

                case "Jump":
                case "Crash":
                    float clamped = Mathf.Clamp(distance, 0f, 5f);
                    Vector2 force = dir * (clamped * 5f);
                    return force / rb.mass;

                default:
                    return Vector2.zero;
            }
        }
    }
}