using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DestructionTest : MonoBehaviour
{
    private ExplosionPooler explosionPooler;
    
    //mouse inputs for getting click position and action
    [SerializeField] private InputActionReference m_pointerInput, m_clickInput;
    //reference to destructible terrain object
    [SerializeField] private DestructibleTerrain m_destructibleTerrain;
    //(optional) explosion effect
    [SerializeField] private GameObject m_explosionEffect;
    //explosion area of effect
    [SerializeField, Min(0.1f)] private float m_radius;

    [SerializeField] private float m_damage = 30f;
    [SerializeField] private float m_knockbackPower = 10f;

    private bool canMakeAction = true;
    public static event Action onMissileExplosion;

    private void Awake()
    {
        explosionPooler = FindAnyObjectByType<ExplosionPooler>();
    }
    
    #region Click Explosion
    private void OnEnable()
    {
        m_clickInput.action.performed += HandleClick;

        TurnManager.onNextTurn += ReactivateClick;
    }
    
    private void OnDisable()
    {
        m_clickInput.action.performed -= HandleClick;

        TurnManager.onNextTurn -= ReactivateClick;
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (!canMakeAction)
            return;

        //gets mouse screen position when clicking
        Vector2 mousePosition = m_pointerInput.action.ReadValue<Vector2>();
        //converts mouse screen position to world position
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Debug.Log(worldPosition);
        //calls destructible terrain function to destroy terrain
        m_destructibleTerrain.DestroyTerrainAt(worldPosition, m_radius);
        
        #region Damage and Knockback
        //overlap sphere at explosion center to damage worms
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, m_radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                Vector2 direction = (worldPosition - (Vector2)collider.transform.position).normalized;
                damageable.TakeDamage(m_damage);
                damageable.Knockback(direction, m_knockbackPower);
            }
        }
        #endregion

        onMissileExplosion?.Invoke();

        //makes sure to go through the wait process before acting again
        canMakeAction = false;

        if (explosionPooler == null)
            return;
        explosionPooler.GetExplosion(worldPosition, Quaternion.identity);
    }

    private void ReactivateClick()
    {
        //allows another explosion to occur
        canMakeAction = true;
    }
    #endregion
}
