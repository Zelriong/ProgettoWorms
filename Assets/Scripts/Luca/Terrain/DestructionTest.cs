using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DestructionTest : MonoBehaviour
{
    //mouse inputs for getting click position and action
    [SerializeField] private InputActionReference m_pointerInput, m_clickInput;
    //reference to destructible terrain object
    [SerializeField] private DestructibleTerrain m_destructibleTerrain;
    //(optional) explosion effect
    [SerializeField] private GameObject m_explosionEffect;
    //explosion area of effect
    [SerializeField, Min(0.1f)] private float m_radius;

    private bool canMakeAction = true;
    public static event Action onMissileExplosion;

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

        onMissileExplosion?.Invoke();

        //makes sure to go through the wait process before acting again
        canMakeAction = false;

        if (m_explosionEffect == null)
            return;
        //generates an explosion effect at clicked position
        Instantiate(m_explosionEffect, worldPosition, Quaternion.identity);
    }

    private void ReactivateClick()
    {
        //allows another explosion to occur
        canMakeAction = true;
    }
    #endregion
}
