using UnityEngine;

public class ExplosionDamage : MonoBehaviour, IDamageable
{
    Rigidbody2D rb;
    
    private float maxHealth = 1000f;
    private float currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Despawn();
        }
    }

    public void Knockback(Vector2 direction, float knockbackPower)
    {
        rb.AddForce(-direction * knockbackPower, ForceMode2D.Impulse);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
