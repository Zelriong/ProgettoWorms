using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private ExplosionPooler explosionPooler;
    private DestructibleTerrain m_destructibleTerrain;
    
    [SerializeField] int damage;
    
    Rigidbody2D rb;
    PlayerMovement player;
    float power;
    
    [Header("Explosion")]
    [SerializeField] private float m_destuctionRadius;
    [SerializeField] private float m_damageRadius;
    [SerializeField] private float m_damage = 30f;
    [SerializeField] private float m_knockbackPower = 10f;
    [SerializeField] private GameObject m_explosionEffect;
    
    public static event Action onMissileExplosion;

    //float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<PlayerMovement>();
        power = player.power;
        
        explosionPooler = FindAnyObjectByType<ExplosionPooler>();
        m_destructibleTerrain = FindAnyObjectByType<DestructibleTerrain>();
    }
    private void OnEnable()
    {
        if (player.direction == 1f)
            rb.AddForce(-transform.up * player.launchDirection * (power/5), ForceMode2D.Impulse);
        else 
            rb.AddForce(-transform.right * player.launchDirection * (power/5), ForceMode2D.Impulse);
    }

    private void Update()
    {
        //timer += Time.deltaTime;

         Vector2 v = rb.linearVelocity;
         float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
         Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle).normalized;
         transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);

         //if (timer > 5) Explode();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }

    private void Explode()
    {
        Vector2 worldPosition = transform.position;
        m_destructibleTerrain.DestroyTerrainAt(worldPosition, m_destuctionRadius);
        
        #region Damage and Knockback
        //overlap sphere at explosion center to damage worms
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, m_damageRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                Vector2 direction = (worldPosition - (Vector2)collider.transform.position).normalized;
                float distance = Vector2.Distance(worldPosition, collider.transform.position);
                if (distance <= 1f)
                    distance = 1f;
                damageable.TakeDamage(m_damage, distance);
                damageable.Knockback(direction, m_knockbackPower, distance);
            }
        }
        #endregion
        
        onMissileExplosion?.Invoke();
        
        

        if (explosionPooler == null)
            return;
        explosionPooler.GetExplosion(worldPosition, Quaternion.identity);

       
    }
}
