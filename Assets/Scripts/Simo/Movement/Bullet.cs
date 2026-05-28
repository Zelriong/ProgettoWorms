using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    

    Rigidbody2D rb;
    PlayerMovement player;
    float power;

    float timer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<PlayerMovement>();
        power = player.power;
    }
    private void OnEnable()
    {
        rb.AddForce(transform.right * (power / 2), ForceMode2D.Impulse);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        Vector2 v = rb.linearVelocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    
        if (timer > 5) Explode();

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider == gameObject.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage(damage, 1f);

            Explode();
        }
    }

    private void Explode()
    {
        Destroy(this);
    }

}
