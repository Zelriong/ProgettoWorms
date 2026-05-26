using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable player))
            player.TakeDamage(100f, 1f);
    }
}
