using System;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{
    public GameObject projectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot(Vector2 cursorPosition)
    {
        Vector2 dir = (cursorPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(cursorPosition, transform.position);

        float speedMultiplier = 5f; 
        float speed = distance * speedMultiplier;

        GameObject proj = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * speed;
        }
    }
}
