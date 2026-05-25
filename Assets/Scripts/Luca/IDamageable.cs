using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage);
    public void Knockback(Vector2 direction, float knockbackPower);
    public void Despawn();
}
