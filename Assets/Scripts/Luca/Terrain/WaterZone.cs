using System;
using UnityEngine;
using System.Collections;

public class WaterZone : MonoBehaviour
{
    [SerializeField] AudioClip[] splish;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(100f, 1f);
            int rand = UnityEngine.Random.Range(0, splish.Length);
            SoundFXManager.instance.PlaySoundFXClip(splish[rand], transform, 1f);
        }
    }
}
