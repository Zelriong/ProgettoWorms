using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TurnManager tm;

    private float totalP1Health = 0f;
    private float totalP2Health = 0f;
    
    private float gameTimer = 0f;
    [SerializeField] private float turnTimer = 30f;
    private float currentTurnTimer;

    public static event Action<float> onGameTimerChange, onTurnTimerChange;
    public static event Action onTurnTimerFinished;
    public static event Action<float, float> onP1HealthUpdated, onP2HealthUpdated;

    private void Awake()
    {
        tm = FindAnyObjectByType<TurnManager>();
    }

    private void OnEnable()
    {
        //occurs at end of Waiting TurnState
        TurnManager.onNextTurn += ResetTurnTimer;
        
        PlayerDamage.onDamageTaken += UpdatePlayerHealth;
    }

    private void OnDisable()
    {
        TurnManager.onNextTurn -= ResetTurnTimer;
        
        PlayerDamage.onDamageTaken -= UpdatePlayerHealth;
    }

    private void Start()
    {
        currentTurnTimer = turnTimer;
        
        GetTotalPlayerHealth();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        onGameTimerChange?.Invoke(gameTimer);

        if (tm.turnState != TurnState.Preparing)
            return;

        currentTurnTimer -= Time.deltaTime;
        onTurnTimerChange?.Invoke(currentTurnTimer);

        if (currentTurnTimer <= 0f)
        {
            TurnTimerFinished();
        }
    }

    private void TurnTimerFinished()
    {
        onTurnTimerFinished?.Invoke();
    }
    private void ResetTurnTimer()
    {
        currentTurnTimer = turnTimer;
    }

    private void GetTotalPlayerHealth()
    {
        foreach (GameObject worm in tm.p1Worms)
        {
            if (worm.TryGetComponent(out PlayerDamage player))
                totalP1Health += player.maxHealth;
        }

        foreach (GameObject worm in tm.p2Worms)
        {
            if (worm.TryGetComponent(out PlayerDamage player))
                totalP2Health += player.maxHealth;
        }
    }

    private void UpdatePlayerHealth(bool p1)
    {
        if (p1)
        {
            float p1Health = 0f;
            
            foreach (GameObject worm in tm.p1Worms)
            {
                if (worm.TryGetComponent(out PlayerDamage player))
                {
                    float health = player.currentHealth;
                    if (player.currentHealth <= 0f)
                        health = 0f;

                    p1Health += health;
                }
            }
            
            onP1HealthUpdated?.Invoke(p1Health, totalP1Health);
        }
        else
        {
            float p2Health = 0f;

            foreach (GameObject worm in tm.p2Worms)
            {
                if (worm.TryGetComponent(out PlayerDamage player))
                {
                    float health = player.currentHealth;
                    if (player.currentHealth <= 0f)
                        health = 0f;

                    p2Health += health;
                }
            }
            
            onP2HealthUpdated?.Invoke(p2Health, totalP2Health);
        }
    }
}
