using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, float distance);
    public void Knockback(Vector2 direction, float knockbackPower, float distance);
    public void Despawn();
}
