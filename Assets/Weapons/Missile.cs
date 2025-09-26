public interface Weapon
{
    
}

public class Missile : Weapon
{
    private float explosionRadius;
    private float explosionForce;
    private int damage;
    public Missile(float explosionRadius, float explosionForce, int damage)
    {
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
        this.damage = damage;
    }
}
