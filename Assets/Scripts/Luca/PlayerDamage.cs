using System;
using UnityEngine;
using TMPro;

public class PlayerDamage : MonoBehaviour, IDamageable, IIndexable
{
    Rigidbody2D rb;
    private TurnManager tm;
    
    public float maxHealth = 100f;
    public float currentHealth;
    
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
        gameObject.SetActive(false);
    }

    public void AssignIndex(int index)
    {
        listIndex = index;
    }
}
