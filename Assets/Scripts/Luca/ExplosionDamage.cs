using UnityEngine;

public class ExplosionDamage : MonoBehaviour, IDamageable, IIndexable
{
    Rigidbody2D rb;
    private TurnManager tm;
    
    private float maxHealth = 100f;
    private float currentHealth;
    
    [SerializeField] private bool isP1;
    [SerializeField] private int listIndex;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tm = FindAnyObjectByType<TurnManager>();
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
        tm.RemoveObjectFromList(isP1, listIndex);
        gameObject.SetActive(false);
    }

    public void AssignIndex(int index)
    {
        listIndex = index;
    }
}
