using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TurnManager tm;

    private float gameTimer = 0f;
    [SerializeField] private float turnTimer = 30f;
    private float currentTurnTimer;

    public static event Action<float> onGameTimerChange, onTurnTimerChange;
    public static event Action onTurnTimerFinished;

    private void Awake()
    {
        tm = FindAnyObjectByType<TurnManager>();
    }

    private void OnEnable()
    {
        //occurs at end of Waiting TurnState
        TurnManager.onNextTurn += ResetTurnTimer;
    }

    private void OnDisable()
    {
        TurnManager.onNextTurn -= ResetTurnTimer;
    }

    private void Start()
    {
        currentTurnTimer = turnTimer;
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
}
