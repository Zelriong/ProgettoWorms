using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerDamage : MonoBehaviour, IDamageable, IIndexable
{
    Rigidbody2D rb;
    private TurnManager tm;
    
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("SFX")]
    [SerializeField] AudioClip[] ouch;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip[] eliminate;

    [SerializeField] private bool isP1;
    [SerializeField] private int listIndex;
    [SerializeField] private TMP_Text healthTxt;

    
    public static event Action<bool> onDamageTaken;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tm = FindAnyObjectByType<TurnManager>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthTxt.text = currentHealth.ToString();
    }

    public void TakeDamage(float damage, float distance)
    {
        currentHealth -= damage / distance;
        healthTxt.text = Mathf.RoundToInt(currentHealth).ToString();
        onDamageTaken?.Invoke(isP1);
        int rand = UnityEngine.Random.Range(0, ouch.Length);
        SoundFXManager.instance.PlaySoundFXClip(ouch[rand], transform, 1f);
        if (currentHealth <= 0)
        {
            Despawn();
        }
    }

    public void Knockback(Vector2 direction, float knockbackPower, float distance)
    {
        rb.AddForce(-direction * (knockbackPower / distance), ForceMode2D.Impulse);
    }

    public void Despawn()
    {
        tm.RemoveObjectFromList(isP1, listIndex);
        SoundFXManager.instance.PlaySoundFXClip(death, transform, 1f);
        int rand = UnityEngine.Random.Range(0, eliminate.Length);
        SoundFXManager.instance.PlaySoundFXClip(eliminate[rand], transform, 1f);
        gameObject.SetActive(false);
    }

    public void AssignIndex(int index)
    {
        listIndex = index;
    }
}
