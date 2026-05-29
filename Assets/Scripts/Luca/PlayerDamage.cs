using System;
using UnityEngine;
using TMPro;

public class PlayerDamage : MonoBehaviour, IDamageable, IIndexable
{
    private TurnManager tm;
    private Bullet bullet;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask mask;
    private float waitForAnimation;
    
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
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        bullet = FindAnyObjectByType<Bullet>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthTxt.text = currentHealth.ToString();
    }

    // private void Update()
    // {
    //     if (IsGrounded() && waitForAnimation > 0f)
    //     {
    //         anim.SetBool("IsTakingDamage", false);
    //     }
    //     else
    //     {
    //         waitForAnimation -= Time.deltaTime;
    //     }
    //     
    // }

    public void TakeDamage(float damage, float distance)
    {
        waitForAnimation += 1f;
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
        bullet.Explode();
        gameObject.SetActive(false);
    }

    public void AssignIndex(int index)
    {
        listIndex = index;
    }
    
    // private bool IsGrounded()
    // {
    //     RaycastHit2D rayHit = Physics2D.CapsuleCast(capsuleCollider2d.bounds.center,
    //         capsuleCollider2d.bounds.size,
    //         0f, 0f, Vector2.down, 0.15f, mask);
    //     return rayHit;
    // }
}
